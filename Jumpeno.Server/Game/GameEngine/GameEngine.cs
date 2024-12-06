namespace Jumpeno.Server.Utils;

public class GameEngine : IUpdateable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Game Game { get; set; }
    public string Code => Game.Code;
    public string Name => Game.Name;
    private readonly Locker GameLock;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameEngine(ACCESS_MODE access, string code, string name, byte capacity) {
        Game = new Game(access, code, name, capacity);
        GameLock = new Locker();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public Game ClientGameCopy() { return Game; }
    
    // Player actions ---------------------------------------------------------------------------------------------------------------------
    public async Task<Player> AddPlayer(User user, bool touchDevice) {
        return await GameLock.Exclusive(async token => {
            var player = Game.AllocatePlayer(user, touchDevice);
            var update = Game.NewPlayerUpdate(player, PLAYER_ACTION.JOIN);
            var updated = Game.Update(update);
            token.Unlock();
            if (updated) await GameHub.SendGameUpdate(Code, update);
            return player;
        });
    }

    public async Task RemovePlayer(Player player) {
        await GameLock.Exclusive(async token => {
            var update = Game.NewPlayerUpdate(player, PLAYER_ACTION.LEAVE);
            var updated = Game.Update(update);
            token.Unlock();
            if (updated) await GameHub.SendGameUpdate(Code, update);
        });
    }

    private Thread? GameLoopThread = null;
    // Controls ---------------------------------------------------------------------------------------------------------------------------
    public async Task Start() {
        await GameLock.Exclusive(async token => {
            if (Game.State == GAME_STATE.GAMEPLAY) return;
            var update = Game.NewStateUpdate(0, GAME_STATE.GAMEPLAY);
            var updated = Game.Update(update);
            GameLoopThread = new Thread(GameLoop);
            GameLoopThread.Start();
            token.Unlock();
            if (updated) await GameHub.SendGameUpdate(Code, Game.NewGamePlayUpdate(update));
        });
    }

    public async Task Reset() {
        await GameLock.Exclusive(async token => {
            ResetKeyUpdates();
            var update = Game.NewStateUpdate(0, GAME_STATE.LOBBY);
            var updated = Game.Update(update);
            token.Unlock();
            if (updated) await GameHub.SendException(Code, new([new("You have been disconnected from the server.")], false));
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
                var movement = Game.NewMovementUpdate(update.PlayerID, update.ID);
                if (loop.Update.Movements.TryGetValue(update.PlayerID, out var previous)) movement.Chain(previous);
                // 3) Add movement update to loop:
                loop.Update.Movements[update.PlayerID] = movement;
            }
            KeyUpdates.Clear();
            loop.Send = true;
        }
        KeyUpdateLock.Unlock();
    }

    private void AddCollisionUpdates(ref (GamePlayUpdate Update, bool Send) loop) {
        foreach (var (player, index) in Game.PlayerIterator) {
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
            // 1) Send update to all clients:
            await GameHub.SendGameUpdate(Code, loop.Update, CLIENT_DEVICE.ALL);
            Game.TouchClock.Reset();
        } else if (Game.Access == ACCESS_MODE.EACH_OWN) {
            // 2) Send periodical (wake-up) updates to touch devices:
            var deltaT = Game.TouchClock.ComputeDelta();
            if (deltaT < Game.TouchClock.IntervalMS) return;
            await GameHub.SendGameUpdate(Code, loop.Update, CLIENT_DEVICE.TOUCH);
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
