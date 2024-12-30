namespace Jumpeno.Server.Utils;

public class GameEngine : IUpdateable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Game Game { get; private set; }
    private DateTime? RoundFinishedAt = null;
    private Thread? GameLoopThread = null;
    private readonly Locker GameLock;
    private readonly EventWaitHandle GameHandle;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameEngine(DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name, byte capacity) {
        Game = new Game(displayMode, mode, host, code, name, capacity);
        GameLock = new Locker();
        GameHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
    }
    
    // Player actions ---------------------------------------------------------------------------------------------------------------------
    public async Task<Player> AddPlayer(Connection connection) {
        return await GameLock.Exclusive(async token => {
            var player = Game.ConnectPlayer(connection);
            var update = Game.NewPlayerUpdate(player);
            var updated = Game.Update(update);
            token.Unlock();
            if (updated) await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
            return player;
        });
    }

    public async Task RemovePlayer(Player player) {
        await GameLock.Exclusive(async token => {
            player.Disconnect();
            var update = Game.NewPlayerUpdate(player);
            var updated = Game.Update(update);
            token.Unlock();
            if (updated) await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
        });
    }

    // Spectator actions ------------------------------------------------------------------------------------------------------------------
    public async Task<Spectator> AddSpectator(Connection connection) {
        return await GameLock.Exclusive(async token => {
            var spectator = Game.ConnectSpectator(connection);
            var update = Game.NewSpectatorUpdate();
            token.Unlock();
            await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
            return spectator;
        });
    }

    public async Task RemoveSpectator(Spectator spectator) {
        await GameLock.Exclusive(async token => {
            Game.RemoveSpectator(spectator);
            var update = Game.NewSpectatorUpdate();
            token.Unlock();
            await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
        });
    }

    // Controls ---------------------------------------------------------------------------------------------------------------------------
    public async Task Start() {
        await GameLock.Exclusive(token => {
            if (!Game.LOBBY_STATES.Contains(Game.State)) return Task.CompletedTask;
            ResetKeyUpdates();
            RoundFinishedAt = null;
            Game.Update(Game.NewStateUpdate(0, GAME_STATE.GAMEPLAY));
            GameLoopThread = new Thread(GameLoop);
            GameLoopThread.Start();
            return Task.CompletedTask;
        });
    }

    private GAME_STATE PausedState = GAME_STATE.GAMEPLAY;
    public async Task Pause() {
        await GameLock.Exclusive(() => {
            if (!Game.RUN_STATES.Contains(Game.State)) return Task.CompletedTask;
            PausedState = Game.State;
            Game.Update(Game.NewStateUpdate(Game.Time, GAME_STATE.PAUSE));
            return Task.CompletedTask;
        });
    }
    public async Task Resume() {
        await GameLock.Exclusive(() => {
            if (!Game.PAUSE_STATES.Contains(Game.State)) return Task.CompletedTask;
            Game.Update(Game.NewStateUpdate(Game.Time, PausedState));
            GameHandle.Set();
            return Task.CompletedTask;
        });
    }

    public async Task Reset() {
        await GameLock.Exclusive(async token => {
            Game.ResetUpdateGuards();
            ResetKeyUpdates();
            RoundFinishedAt = null;
            var update = Game.NewStateUpdate(0, GAME_STATE.LOBBY);
            var updated = Game.Update(update);
            GameHandle.Set();
            token.Unlock();
            if (updated) await GameHub.SendException(Game, UPDATE_GROUP.ALL, new(new Error("You have been disconnected from the server.")));
        });
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update) {
        if (update is KeyUpdate key) return KeyUpdate(key);
        return false;
    }

    private readonly LinkedList<KeyUpdate> KeyUpdates = [];
    private readonly Locker KeyUpdateLock = new();
    private bool KeyUpdate(KeyUpdate update) => KeyUpdateLock.Exclusive(() => { KeyUpdates.AddLast(update); return true; });
    private void ResetKeyUpdates() => KeyUpdateLock.Exclusive(KeyUpdates.Clear);

    // Loop methods -----------------------------------------------------------------------------------------------------------------------
    private async Task StartRound() {
        var update = Game.NewRoundStartUpdate();
        Game.Update(update);
        await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
    }

    private async Task HandlePause() {
        if (!Game.PAUSE_STATES.Contains(Game.State)) return;
        await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, Game.NewGamePlayCurrentUpdate());
        GameLock.Unlock();
        GameHandle.WaitOne();
        GameLock.Lock();
        await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, Game.NewGamePlayCurrentUpdate());
        ResetKeyUpdates();
    }

    private async Task UpdateRound() {
        await GameLock.Exclusive(async () => {
            if (Game.State == GAME_STATE.LOBBY) return;
            var update = Game.NewRoundFinishUpdate();
            Game.Update(update);
            await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
        });
    }

    private void EvaluatePlayerCollisions(ref (GamePlayUpdate Update, bool Send) loop) {
        // TODO: [Optional] Implement continuous collision detection (will increase complexity!)
        foreach (var (player, index) in Game.PlayerIterator) {
            var colliding = Game.GetCollidingPlayers(player);
            foreach (var other in colliding) {
                // 1) Check self:
                if (player.Equals(other)) continue;
                // 2) Check jump:
                if (!player.JumpedOn(other)) continue;
                // 3) Kill update:
                var killUpdate = Game.NewKillUpdate(player.ID, other.ID);
                Game.Update(killUpdate);
                // 4) Movement update:
                var movement = new MovementUpdate(
                    player.ID,
                    new(player.Body.Position.Center.X, other.Body.Position.Center.Y + 2 * Body.HALF_HEIGHT),
                    player.Body.Direction, player.Body.JumpFinishY
                );
                Game.Update(movement);
                // 5) Set loop updates:
                loop.Update.Movements[player.ID] = movement;
                loop.Update.Movements[other.ID] = Game.NewMovementUpdate(other.ID);
                loop.Update.Kills[other.ID] = killUpdate;
                loop.Send = true;
            }
        }
    }

    private void ApplyKeyUpdates(ref (GamePlayUpdate Update, bool Send) loop) {
        KeyUpdateLock.Lock();
        if (KeyUpdates.Count > 0) {
            foreach (var update in KeyUpdates) {
                // 1) Update game:
                Game.Update(update);
                // 2) Generate movement update:
                var movement = Game.NewMovementUpdate(update.PlayerID);
                // 3) Add movement update to loop:
                loop.Update.Movements[update.PlayerID] = movement;
            }
            KeyUpdates.Clear();
            loop.Send = true;
        }
        KeyUpdateLock.Unlock();
    }

    private void AddLifeUpdates(ref (GamePlayUpdate Update, bool Send) loop) {
        if (Game.Mode != GAME_MODE.MAYHEM) return;
        foreach (var (player, _) in Game.PlayerIterator) {
            if (player.Body.Alive || !player.Body.Fallen) continue;
            var lifeUpdate = Game.NewLifeUpdate(player);
            Game.Update(lifeUpdate);
            loop.Update.Lives[player.ID] = lifeUpdate;
            loop.Send = true;
        }
    }

    private void AddCollisionUpdates(ref (GamePlayUpdate Update, bool Send) loop) {
        foreach (var (player, _) in Game.PlayerIterator) {
            // 1) Check collision:
            if (!player.CollisionDetected) continue;
            // 2) Check movement update:
            if (loop.Update.Movements.ContainsKey(player.ID)) continue;
            // 3) Add movement update to loop:
            loop.Update.Movements[player.ID] = Game.NewMovementUpdate(player.ID);
            loop.Send = true;
        }
    }

    private void TryFinishGame(ref (GamePlayUpdate Update, bool Send) loop) {
        if (RoundFinishedAt is not DateTime finishTime) {
            switch (Game.Mode) {
                case GAME_MODE.LAST_STANDING:
                    if (Game.AlivePlayerCount > 1) return;
                break;
                case GAME_MODE.MAYHEM:
                    if (Game.Time < Game.ROUND_DURATION) return;
                break;
            }
            RoundFinishedAt = DateTime.UtcNow;
        } else if (Game.Mode == GAME_MODE.MAYHEM || GameClock.DeltaAhead(finishTime) >= Game.ROUND_FINISH_DELAY) {
            Game.Update(Game.NewStateUpdate(Game.Time, GAME_STATE.SCOREBOARD));
            loop.Send = false;
        }
    }

    private async Task SendLoopUpdate((GamePlayUpdate Update, bool Send) loop) {
        if (loop.Send) {
            // 1) If killed or alive, send update to everyone (sync score & stop sending updates on joysticks):
            if (Game.DisplayMode == DISPLAY_MODE.ONE_SCREEN && (loop.Update.Kills.Count > 0 || loop.Update.Lives.Count > 0)) {
                await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, loop.Update);
            // 2) Send update to all watching clients:
            } else await GameHub.SendGameUpdate(Game, UPDATE_GROUP.WATCH, loop.Update);
            Game.TouchClock.Reset();
        } else {
            // 3) Send periodical (wake-up) updates to touch devices:
            var deltaT = Game.TouchClock.ComputeDelta();
            if (deltaT < Game.TouchClock.IntervalMS) return;
            await GameHub.SendGameUpdate(Game, UPDATE_GROUP.WATCH_TOUCH, loop.Update);
            Game.TouchClock.Update(deltaT);
        }
    }

    // Game loop --------------------------------------------------------------------------------------------------------------------------
    private async void GameLoop() {
        // 1) Initial update:
        await StartRound();

        // 2) Run loop:
        while (true) {
            // 2.1) Await delta of time:
            var deltaT = await Game.Clock.AwaitDelta();

            // 2.2) Update game:
            if (await GameLock.Exclusive(async () => {
                // 2.2.1) Return true if game finished:
                if (Game.LOBBY_STATES.Contains(Game.State)) return true;
                // 2.2.2) Pause and repeat loop:
                if (Game.PAUSE_STATES.Contains(Game.State)) { await HandlePause(); return false; }

                // 2.2.3) Prepare loop update:
                var loop = (Update: Game.NewGamePlayCurrentUpdate(), Send: false);

                // 2.2.4) Move game objects:
                Game.Update(Game.NewTimeFlowUpdate(deltaT));
                // 2.2.5) Evaluate player interactions:
                EvaluatePlayerCollisions(ref loop);
                // 2.2.6) Apply key updates:
                ApplyKeyUpdates(ref loop);
                // 2.2.7) Add life updates:
                AddLifeUpdates(ref loop);
                // 2.2.8) Add movement corrections:
                AddCollisionUpdates(ref loop);
                // 2.2.9) Try to finish:
                TryFinishGame(ref loop);

                // 2.2.10) Send loop update:
                await SendLoopUpdate(loop);

                // 2.2.11) Game not finished:
                return false;
            })) break;
        }

        // 3) Finish round:
        await UpdateRound();
        GameLoopThread = null;
    }
}
