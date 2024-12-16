namespace Jumpeno.Server.Utils;

public class GameEngine : IUpdateable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Game Game { get; private set; }
    private Thread? GameLoopThread = null;
    private readonly Locker GameLock;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameEngine(DISPLAY_MODE display, User host, string code, string name, byte capacity) {
        Game = new Game(display, host, code, name, capacity);
        GameLock = new Locker();
    }
    
    // Player actions ---------------------------------------------------------------------------------------------------------------------
    public async Task<Player> AddPlayer(User user, DEVICE_TYPE device) {
        return await GameLock.Exclusive(async token => {
            var player = Game.AllocatePlayer(user, device);
            var update = Game.NewPlayerUpdate(player, PLAYER_ACTION.JOIN);
            var updated = Game.Update(update);
            token.Unlock();
            if (updated) await GameHub.SendGameUpdate(Game.Code, UPDATE_GROUP.ALL, update);
            return player;
        });
    }

    public async Task RemovePlayer(Player player) {
        await GameLock.Exclusive(async token => {
            var update = Game.NewPlayerUpdate(player, PLAYER_ACTION.LEAVE);
            var updated = Game.Update(update);
            token.Unlock();
            if (updated) await GameHub.SendGameUpdate(Game.Code, UPDATE_GROUP.ALL, update);
        });
    }

    // Spectator actions ------------------------------------------------------------------------------------------------------------------
    public async Task<Spectator> AddSpectator(User user, DEVICE_TYPE device) {
        return await GameLock.Exclusive(async token => {
            var spectator = Game.AddSpectator(user, device);
            var update = Game.NewWatchUpdate();
            token.Unlock();
            await GameHub.SendGameUpdate(Game.Code, UPDATE_GROUP.ALL, update);
            return spectator;
        });
    }

    public async Task RemoveSpectator(Spectator spectator) {
        await GameLock.Exclusive(async token => {
            Game.RemoveSpectator(spectator);
            var update = Game.NewWatchUpdate();
            token.Unlock();
            await GameHub.SendGameUpdate(Game.Code, UPDATE_GROUP.ALL, update);
        });
    }

    // Controls ---------------------------------------------------------------------------------------------------------------------------
    public async Task Start() {
        await GameLock.Exclusive(async token => {
            if (Game.State == GAME_STATE.GAMEPLAY) return;
            var update = Game.NewStateUpdate(0, GAME_STATE.GAMEPLAY);
            var updated = Game.Update(update);
            GameLoopThread = new Thread(GameLoop);
            GameLoopThread.Start();
            token.Unlock();
            if (updated) await GameHub.SendGameUpdate(Game.Code, UPDATE_GROUP.ALL, Game.NewGamePlayUpdate(update));
        });
    }

    public async Task Reset() {
        await GameLock.Exclusive(async token => {
            ResetKeyUpdates();
            var update = Game.NewStateUpdate(0, GAME_STATE.LOBBY);
            var updated = Game.Update(update);
            token.Unlock();
            if (updated) await GameHub.SendException(Game.Code, UPDATE_GROUP.ALL, new(new Error("You have been disconnected from the server.")));
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
    private void ResetKeyUpdates() => KeyUpdateLock.Exclusive(() => KeyUpdates.Clear());

    // Loop methods -----------------------------------------------------------------------------------------------------------------------
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
                loop.Update.Kills.Add(other.ID, killUpdate);
                // 4) Mark send:
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

    private async Task SendLoopUpdate((GamePlayUpdate Update, bool Send) loop) {
        if (loop.Send) {
            // 1) Send update to all watching clients:
            await GameHub.SendGameUpdate(Game.Code, UPDATE_GROUP.WATCH, loop.Update);
            Game.TouchClock.Reset();
        } else {
            // 2) Send periodical (wake-up) updates to touch devices:
            var deltaT = Game.TouchClock.ComputeDelta();
            if (deltaT < Game.TouchClock.IntervalMS) return;
            await GameHub.SendGameUpdate(Game.Code, UPDATE_GROUP.WATCH_TOUCH, loop.Update);
            Game.TouchClock.Update(deltaT);
        }
    }

    // Game loop --------------------------------------------------------------------------------------------------------------------------
    private async void GameLoop() {        
        // 1) Run loop:
        while (Game.State == GAME_STATE.GAMEPLAY) {
            // 1.1) Await delta of time:
            var deltaT = await Game.Clock.AwaitDelta();

            GameLock.Lock();
            // 1.2) Prepare loop update:
            var loop = (Update: Game.NewGamePlayUpdate(Game.NewStateUpdate(Game.Time, Game.State)), Send: false);
            // 1.3) Move game objects
            Game.Update(Game.NewTimeFlowUpdate(deltaT));
            // 1.4) Evaluate player interactions:
            EvaluatePlayerCollisions(ref loop);
            // 1.5) Apply key updates:
            ApplyKeyUpdates(ref loop);
            // 1.6) Add movement corrections:
            AddCollisionUpdates(ref loop);
            GameLock.Unlock();

            // 1.7) Send loop update:
            await SendLoopUpdate(loop);
        }
        // 2) Release resources:
        GameLoopThread = null;
    }
}
