namespace Jumpeno.Server.Utils;

#pragma warning disable CS1998

public class GameEngine : IUpdateable, IDisposable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly Game Game;
    // Loop:
    private double? RoundFinishTimer = null;
    private Thread? GameLoopThread = null;
    // Locks:
    private readonly Locker GameLock;
    private readonly EventWaitHandle GameHandle;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameEngine(DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name, Map map, byte capacity) {
        Game = new Game(displayMode, mode, host, code, name, map, capacity);
        GameLock = new Locker();
        GameHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
    }

    public void Dispose() {
        GameLock.Lock();
        GameHandle.Set();
        KeyUpdateLock.Lock();
        GameLock.Dispose();
        GameHandle.Dispose();
        KeyUpdateLock.Dispose();
        GC.SuppressFinalize(this);
    }

    // Player actions ---------------------------------------------------------------------------------------------------------------------
    public async Task<GameContext> AddPlayer(Connection connection) {
        return await GameLock.Exclusive(async () => {
            // 1) Before event:
            await GameHub.BeforeConnected();
            // 2) Connect player:
            var player = Game.ConnectPlayer(connection);
            var update = Game.NewPlayerUpdate(player);
            var updated = Game.Update(update);
            if (updated) await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
            // 3) Create context:
            var context = new GameContext(this, player);
            // 4) After event:
            await GameHub.AfterConnected(context); return context;
        });
    }

    public async Task RemovePlayer(Player player) {
        await GameLock.Exclusive(async () => {
            // 1) Before event:
            var context = new GameContext(this, player);
            await GameHub.BeforeDisconnected(context);
            // 2) Disconnect player:
            player.Disconnect();
            var update = Game.NewPlayerUpdate(player);
            var updated = Game.Update(update);
            if (updated) await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
            // 3) After event:
            await GameHub.AfterDisconnected(context);
        });
    }

    // Spectator actions ------------------------------------------------------------------------------------------------------------------
    public async Task<GameContext> AddSpectator(Connection connection) {
        return await GameLock.Exclusive(async () => {
            // 1) Before event:
            await GameHub.BeforeConnected();
            // 2) Connect spectator:
            var spectator = Game.ConnectSpectator(connection);
            var update = Game.NewSpectatorUpdate();
            await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
            // 3) Create context:
            var context = new GameContext(this, spectator);
            // 4) After event:
            await GameHub.AfterConnected(context);
            return context;
        });
    }

    public async Task RemoveSpectator(Spectator spectator) {
        await GameLock.Exclusive(async () => {
            // 1) Before event:
            var context = new GameContext(this, spectator);
            await GameHub.BeforeDisconnected(context);
            // 2) Disconnect spectator:
            Game.Update(Game.NewSpectatorRemoveUpdate());
            var update = Game.NewSpectatorUpdate();
            await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
            // 3) After event:
            await GameHub.AfterDisconnected(context);
        });
    }

    // Game actions -----------------------------------------------------------------------------------------------------------------------
    public async Task Start() {
        GameLock.Exclusive(() => {
            if (!Game.LOBBY_STATES.Contains(Game.State)) return;
            GameLoopThread = new Thread(GameLoop);
            GameLoopThread.Start();
        });
    }

    public async Task Pause() {
        GameLock.Exclusive(() => {
            if (!Game.RUN_STATES.Contains(Game.State)) return;
            PausedState = Game.State;
            Game.Update(Game.NewStateUpdate(Game.Time, GAME_STATE.PAUSE));
        });
    }
    private GAME_STATE PausedState = GAME_STATE.GAMEPLAY;
    public async Task Resume() {
        GameLock.Exclusive(() => {
            if (!Game.PAUSE_STATES.Contains(Game.State)) return;
            Game.Update(Game.NewStateUpdate(Game.Time, PausedState));
            GameHandle.Set();
        });
    }

    public async Task Reset() {
        await GameLock.Exclusive(async () => {
            RoundFinishTimer = null;
            ResetKeyUpdates();
            Game.Update(Game.NewRoundResetUpdate());
            GameHandle.Set();
            await GameHub.SendException(Game, UPDATE_GROUP.ALL, EXCEPTION.DISCONNECT);
        });
    }

    // Loop [Round] -----------------------------------------------------------------------------------------------------------------------
    private async Task<bool> StartRound() {
        return await GameLock.Exclusive(async () => {
            // 1) Do not run loop if not in lobby:
            if (!Game.LOBBY_STATES.Contains(Game.State)) return false;
            // 2) Reset round:
            RoundFinishTimer = null;
            ResetKeyUpdates();
            // 3) Start round:
            var update = Game.NewRoundStartUpdate();
            Game.Update(update);
            // 4) Send round update:
            await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
            // 5) Return true to run loop:
            return true;
        });
    }

    private async Task HandlePause() {
        // 1) Check pause state & notify clients:
        if (!Game.PAUSE_STATES.Contains(Game.State)) return;
        await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, Game.NewGamePlayCurrentUpdate());
        // 2) Wait for resume signal:
        GameLock.Unlock();
        GameHandle.WaitOne();
        GameLock.Lock();
        // 3) Reset key updates during pause:
        ResetKeyUpdates();
        // 4) Check run state & notify clients:
        if (!Game.RUN_STATES.Contains(Game.State)) return;
        await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, Game.NewGamePlayCurrentUpdate());
    }

    private async Task UpdateRound() {
        await GameLock.Exclusive(async () => {
            // 1) Check state:
            if (Game.State != GAME_STATE.SCOREBOARD) return;
            // 2) Update round:
            var update = Game.NewRoundFinishUpdate();
            Game.Update(update);
            // 3) Send round update:
            await GameHub.SendGameUpdate(Game, UPDATE_GROUP.ALL, update);
        });
    }

    // Loop [Shrinking] -------------------------------------------------------------------------------------------------------------------
    private bool ApplyShrinking() {
        // 1) Check state:
        if (Game.Time < Game.ROUND_DURATION || Game.Map.Shrink.Timer < Shrink.DURATION) return false;
        if (Game.Map.Shrink.Level >= 0 && Game.AlivePlayersCount <= 1) return false;
        if (Game.Map.Shrink.Level >= Shrink.MAX_LEVEL) return false;
        // 2) Apply:
        var update = Game.NewStateUpdate(Game.Time, GAME_STATE.SHRINKING, Game.Map.Shrink.Level + 1, 0);
        Game.Update(update);
        return true;
    }

    private void EvaluateShrinking(ref (GamePlayUpdate Update, bool Send) loop) {
        if (Game.State != GAME_STATE.SHRINKING) return;
        foreach (var (player, _) in Game.PlayerIterator) {
            // 1) Check collision:
            if (!player.IsShrinked(Game.Map.Shrink)) continue;
            // 2) Kill update:
            var killUpdate = Game.NewKillUpdate(null, player.ID, true);
            Game.Update(killUpdate);
            // 3) Add kill update to loop:
            loop.Update.Kills[player.ID] = killUpdate;
            loop.Send = true;
        }
    }

    // Loop [Key updates] -----------------------------------------------------------------------------------------------------------------
    private readonly LinkedList<KeyUpdate> KeyUpdates = [];
    private readonly Locker KeyUpdateLock = new();
    private void ResetKeyUpdates() => KeyUpdateLock.Exclusive(KeyUpdates.Clear);

    public bool Update(GameUpdate update) {
        if (update is KeyUpdate key) return KeyUpdate(key);
        return false;
    }

    private bool KeyUpdate(KeyUpdate update) => KeyUpdateLock.Exclusive(() => {
        // 1) Check whether game runs:
        if (!Game.RUN_STATES.Contains(Game.State)) return false;
        // 2) Add update to be applied:
        KeyUpdates.AddLast(update);
        return true;
    });

    private void ApplyKeyUpdates(ref (GamePlayUpdate Update, bool Send) loop) {
        try { KeyUpdateLock.Lock();
            foreach (var update in KeyUpdates) {
                // 1) Update game:
                Game.Update(update);
                // 2) Add movement update to loop:
                loop.Update.Movements[update.PlayerID] = Game.NewMovementUpdate(update.PlayerID);
                // 3) Mark send:
                loop.Send = true;
            }
            KeyUpdates.Clear();
        } finally { KeyUpdateLock.Unlock(); }
    }

    // Loop [Kills & Lives] ---------------------------------------------------------------------------------------------------------------
    private void EvaluatePlayerCollisions(ref (GamePlayUpdate Update, bool Send) loop) {
        // TODO: [Optional] Implement continuous collision detection (will increase complexity!)
        foreach (var (player, _) in Game.PlayerIterator) {
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
                var movement = Game.NewMovementUpdate(player, other);
                Game.Update(movement);
                // 5) Set loop updates:
                loop.Update.Movements[player.ID] = movement;
                loop.Update.Movements[other.ID] = Game.NewMovementUpdate(other.ID);
                loop.Update.Kills[other.ID] = killUpdate;
                loop.Send = true;
            }
        }
    }

    private void AddLifeUpdates(ref (GamePlayUpdate Update, bool Send) loop) {
        // 1) Check mode & state:
        if (Game.Mode != GAME_MODE.MAYHEM || Game.State == GAME_STATE.SHRINKING) return;
        if (RoundFinishTimer != null) return;
        // 2) Add life updates:
        foreach (var (player, _) in Game.PlayerIterator) {
            if (player.Body.Alive || !player.Body.Fallen) continue;
            var lifeUpdate = Game.NewLifeUpdate(player);
            Game.Update(lifeUpdate);
            loop.Update.Lives[player.ID] = lifeUpdate;
            loop.Send = true;
        }
    }

    private void EnforceMoveUpdates(ref (GamePlayUpdate Update, bool Send) loop, bool shrink) {
        foreach (var (player, _) in Game.PlayerIterator) {
            // 1) Check collision:
            if (!shrink && !player.CollisionDetected) continue;
            // 2) Check movement update:
            if (loop.Update.Movements.ContainsKey(player.ID)) continue;
            // 3) Add movement update to loop:
            loop.Update.Movements[player.ID] = Game.NewMovementUpdate(player.ID);
            loop.Send = true;
        }
    }

    // Loop [Finishing] -------------------------------------------------------------------------------------------------------------------
    private void TryFinishGame(ref (GamePlayUpdate Update, bool Send) loop, double deltaT) {
        // 1) Set Timer:
        if (RoundFinishTimer is not double finishTime) {
            if (Game.Mode == GAME_MODE.MAYHEM && Game.Time < Game.ROUND_DURATION && Game.ActivePlayersCount > 1) return;
            if (Game.AlivePlayersCount > 1) return;
            RoundFinishTimer = 0;
        // 2) Increment timer:
        } else if (finishTime < Game.ROUND_FINISH_DELAY) {
            RoundFinishTimer += deltaT;
        // 3) Finish game:
        } else {
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

    // Loop -------------------------------------------------------------------------------------------------------------------------------
    private async void GameLoop() {
        try {
            // 1) Initial update:
            var run = await StartRound();

            // 2) Run loop:
            while (run) {
                // 2.1) Await time delta:
                var deltaT = await Game.Clock.AwaitDelta();

                // 2.2) Update game:
                if (await GameLock.Exclusive(async () => {
                    // 2.2.1) Return true if game finished:
                    if (Game.LOBBY_STATES.Contains(Game.State)) return true;
                    // 2.2.2) Pause and repeat loop:
                    if (Game.PAUSE_STATES.Contains(Game.State)) { await HandlePause(); return false; }

                    // 2.2.3) Prepare loop update & shrink:
                    var shrink = ApplyShrinking();
                    var loop = (Update: Game.NewGamePlayCurrentUpdate(), Send: shrink);
                    EvaluateShrinking(ref loop);

                    // 2.2.4) Move game objects:
                    Game.Update(Game.NewTimeFlowUpdate(deltaT));
                    // 2.2.5) Evaluate player interactions:
                    EvaluatePlayerCollisions(ref loop);
                    // 2.2.6) Apply key updates:
                    ApplyKeyUpdates(ref loop);
                    // 2.2.7) Add life updates:
                    AddLifeUpdates(ref loop);
                    // 2.2.8) Add movement corrections:
                    EnforceMoveUpdates(ref loop, shrink);
                    // 2.2.9) Try to finish:
                    TryFinishGame(ref loop, deltaT);

                    // 2.2.10) Send update & repeat loop:
                    await SendLoopUpdate(loop); return false;
                })) break;
            }

            // 3) Finish round:
            await UpdateRound();
        } catch {
            await GameHub.SendException(Game, UPDATE_GROUP.ALL, EXCEPTION.DISCONNECT);
        } finally {
            GameLoopThread = null;
        }
    }
}
