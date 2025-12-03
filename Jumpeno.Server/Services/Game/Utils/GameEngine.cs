namespace Jumpeno.Server.Utils;

#pragma warning disable CS1998
#pragma warning disable IDE0290

public class GameEngine : IUpdateable, IAsyncDisposable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly Game Game;
    // Loop:
    private double? RoundFinishTimer = null;
    private Thread? GameLoopThread = null;
    // Locks:
    private readonly Locker GameLock = new();
    private readonly EventWaitHandle GameStartHandle = new(false, EventResetMode.AutoReset);
    private readonly EventWaitHandle GameHandle = new(false, EventResetMode.AutoReset);
    // Actions:
    private bool IsPausing = false;
    private bool IsDeleted = false;

    // Hub --------------------------------------------------------------------------------------------------------------------------------
    private async Task SendGameUpdate(UPDATE_GROUP group, NetworkUpdate update) => await GameHub.SendGameUpdate(Game, group, update);

    private async Task SendGameActionUpdate(UPDATE_GROUP group, NetworkUpdate update) {
        SetActionResponse(update);
        await GameHub.SendGameUpdate(Game, group, update);
    }

    private async Task SendException(UPDATE_GROUP group, AppException e) => await GameHub.SendException(Game, group, e);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameEngine(
        DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name,
        Map map, bool anonyms, byte rounds, byte capacity
    )
    => Game = new Game(displayMode, mode, host, code, name, map, anonyms, rounds, capacity);

    public async ValueTask DisposeAsync() {
        // 1) Locks:
        GameLock.Lock();
        KeyUpdateLock.Lock();
        // 2) Dispose handles:
        GameStartHandle.Set();
        GameStartHandle.Dispose();
        GameHandle.Set();
        GameHandle.Dispose();
        // 3) Dispose locks:
        KeyUpdateLock.DisposeUnsafe();
        GameLock.DisposeUnsafe();
        // 4) Finalize:
        GC.SuppressFinalize(this);
    }

    // Player actions ---------------------------------------------------------------------------------------------------------------------
    public async Task<GameContext> AddPlayer(
        // Parameters:
        Connection connection,
        // Exceptions:
        string nameID = ""
    )
    => await GameLock.Exclusive(async () => {
        // 1) Validation:
        if (IsDeleted) throw EXCEPTION.DEFAULT.SetInfo("Game is deleted!");
        // 2) Before event:
        await GameHub.BeforeConnected();
        // 3) Connect player:
        var player = Game.CreatePlayer(connection, nameID);
        // 4) Send update:
        var update = Game.NewPlayerAddUpdate(player);
        var updated = Game.Update(update);
        if (updated) await SendGameUpdate(UPDATE_GROUP.ALL, update);
        // 5) Create context:
        var context = new GameContext(this, player);
        // 6) After event:
        await GameHub.AfterConnected(context); return context;
    });

    public async Task RemovePlayer(Player player)
    => await GameLock.Exclusive(async () => {
        // 1) Before event:
        var context = new GameContext(this, player);
        await GameHub.BeforeDisconnected(context);
        // 2) Disconnect player:
        player.Disconnect();
        // 3) Send update:
        var update = Game.NewPlayerRemoveUpdate(player);
        var updated = Game.Update(update);
        if (updated) await SendGameUpdate(UPDATE_GROUP.ALL, update);
        // 4) After event:
        await GameHub.AfterDisconnected(context);
    });

    // Spectator actions ------------------------------------------------------------------------------------------------------------------
    public async Task<GameContext> AddSpectator(
        // Parameters:
        Connection connection,
        // Exceptions:
        string nameID = ""
    )
    => await GameLock.Exclusive(async () => {
        // 1) Validation:
        if (IsDeleted) throw EXCEPTION.DEFAULT.SetInfo("Game is deleted!");
        // 2) Before event:
        await GameHub.BeforeConnected();
        // 3) Connect spectator:
        var spectator = Game.CreateSpectator(connection, nameID);
        // 4) Send update:
        var update = Game.NewSpectatorAddUpdate(spectator);
        var updated = Game.Update(update);
        if (updated) await SendGameUpdate(UPDATE_GROUP.ALL, update);
        // 5) Create context:
        var context = new GameContext(this, spectator);
        // 6) After event:
        await GameHub.AfterConnected(context); return context;
    });

    public async Task RemoveSpectator(Spectator spectator)
    => await GameLock.Exclusive(async () => {
        // 1) Before event:
        var context = new GameContext(this, spectator);
        await GameHub.BeforeDisconnected(context);
        // 2) Disconnect spectator:
        spectator.Disconnect();
        // 3) Send update:
        var update = Game.NewSpectatorRemoveUpdate(spectator);
        var updated = Game.Update(update);
        if (updated) await SendGameUpdate(UPDATE_GROUP.ALL, update);
        // 4) After event:
        await GameHub.AfterDisconnected(context);
    });

    // Game actions -----------------------------------------------------------------------------------------------------------------------
    public async Task Start(Connection? connection = null)
    => await ActionCall(connection, async () => {
        // 1) Validation:
        if (IsDeleted) throw EXCEPTION.DEFAULT.SetInfo("Game is deleted!");
        // 2) Perform action:
        if (Game.LOBBY_STATES.Contains(Game.State)) await LoopStart();
        else if (Game.PAUSE_STATES.Contains(Game.State)) await LoopContinue();
        // 3) Or throw:
        else throw EXCEPTION.DEFAULT.SetInfo("Game is running!");
    });

    public async Task Pause(Connection? connection = null)
    => await ActionCall(connection, async () => {
        // 1) Validation:
        if (IsDeleted) throw EXCEPTION.DEFAULT.SetInfo("Game is deleted!");
        // 2) Perform action:
        if (Game.RUN_STATES.Contains(Game.State)) await LoopPause();
        // 3) Or throw:
        else throw EXCEPTION.DEFAULT.SetInfo("Game is not running!");
    }, async () => {
        // 4) Send response if pausing:
        if (IsPausing) await SendGameActionUpdate(UPDATE_GROUP.ALL, Game.NewGamePlayCurrentUpdate());
    });

    public async Task Toggle(Connection? connection = null)
    => await ActionCall(connection, async () => {
        // 1) Validation:
        if (IsDeleted) throw EXCEPTION.DEFAULT.SetInfo("Game is deleted!");
        // 2) Perform action:
        if (Game.LOBBY_STATES.Contains(Game.State)) await LoopStart();
        else if (Game.PAUSE_STATES.Contains(Game.State)) await LoopContinue();
        else if (Game.RUN_STATES.Contains(Game.State)) await LoopPause();
        // 3) Or throw:
        else throw EXCEPTION.DEFAULT.SetInfo("Invalid game state!");
    });

    public async Task Delete()
    => await GameLock.Exclusive(async () => {
        // 1) Validation:
        if (IsDeleted) throw EXCEPTION.DEFAULT.SetInfo("Game already deleted!");
        // 2) Set deleted:
        IsDeleted = true;
        // 3) Notify clients:
        await SendException(UPDATE_GROUP.ALL, EXCEPTION.DISCONNECT);
    });

    // Loop [Action][Response] ------------------------------------------------------------------------------------------------------------
    private LinkedList<string> ActionResponseIDs = [];

    private void SetActionResponse(NetworkUpdate update) {
        // 1) Check if needed:
        if (ActionResponseIDs.Count <= 0) return;
        // 2) Set update response:
        update.ResponseIDs = ActionResponseIDs;
        // 3) Clear IDs:
        ActionResponseIDs = [];
    }

    private async Task ActionCall(Connection? connection, Func<Task> action, Func<Task>? after = null) 
    => await GameLock.Exclusive(async () => {
        // 1) Execute action:
        await action();
        // 2) Check connection:
        if (connection?.ConnectionID == null) return;
        // 3) Save response ID:
        ActionResponseIDs.AddLast(connection.ConnectionID);
        // 4) Execute after action:
        if (after != null) await after();
    });

    private void CheckActionResponse(ref (GamePlayUpdate Update, bool Send) loop) {
        if (ActionResponseIDs.Count > 0) loop.Send = true;
    }

    // Loop [Action][Execution] -----------------------------------------------------------------------------------------------------------
    private async Task LoopStart() {
        // 1) Validation:
        if (Game.IsFinished) throw EXCEPTION.DEFAULT.SetInfo("Game is finished!");
        if (GameLoopThread != null) throw EXCEPTION.DEFAULT.SetInfo("Game is running!");
        if (!Game.LOBBY_STATES.Contains(Game.State)) throw EXCEPTION.DEFAULT.SetInfo("Invalid game state!");
        if (Game.ActivePlayersCount < 2) throw EXCEPTION.DEFAULT.SetInfo("At least 2 players required to start!");
        // 2) Start loop:
        try {
            // 2.1) Start thread:
            GameLoopThread = new Thread(GameLoop);
            GameLoopThread.Start();
        } catch {
            // 2.2) Handle exception:
            throw EXCEPTION.DEFAULT.SetInfo("Server can not start the game now!");
        }
        // 3) Start round:
        await StartRound();
    }

    private GAME_STATE PausedState = GAME_STATE.GAMEPLAY;
    private async Task LoopPause() {
        // 1) Save state:
        PausedState = Game.State;
        // 2) Update game:
        Game.Update(Game.NewStateUpdate(Game.Time, GAME_STATE.PAUSE));
        // 3) Reset handle:
        GameHandle.Reset();
    }

    private async Task LoopContinue() {
        // 1) Update game:
        Game.Update(Game.NewStateUpdate(Game.Time, PausedState));
        // 2) Set handle:
        GameHandle.Set();
    }

    // Loop [Round] -----------------------------------------------------------------------------------------------------------------------
    private async Task StartRound() {
        // 1) Reset round:
        RoundFinishTimer = null;
        ResetKeyUpdates();
        // 2) Start round:
        var update = Game.NewRoundStartUpdate();
        Game.Update(update);
        // 3) Send round update:
        await SendGameActionUpdate(UPDATE_GROUP.ALL, update);
        // 4) Set loop handle:
        GameStartHandle.Set();
    }

    private async Task HandlePause() {
        // 1) Check pause state & notify clients:
        if (!Game.PAUSE_STATES.Contains(Game.State)) return;
        await SendGameActionUpdate(UPDATE_GROUP.ALL, Game.NewGamePlayCurrentUpdate());
        // 2) Mark pausing:
        IsPausing = true;
        // 3) Wait for resume signal:
        GameLock.Unlock();
        GameHandle.WaitOne();
        GameLock.Lock();
        // 4) Mark pausing:
        IsPausing = false;
        // 5) Reset key updates during pause:
        ResetKeyUpdates();
        // 6) Check run state & notify clients:
        if (!Game.RUN_STATES.Contains(Game.State)) return;
        await SendGameActionUpdate(UPDATE_GROUP.ALL, Game.NewGamePlayCurrentUpdate());
    }

    private async Task FinishRound() {
        // 1) Update round:
        var update = Game.NewRoundFinishUpdate();
        Game.Update(update);
        // 2) Send round update:
        await SendGameActionUpdate(UPDATE_GROUP.ALL, update);
        // 3) Release thread:
        ReleaseThread();
    }

    private void ReleaseThread() {
        GameLoopThread = null;
        GameStartHandle.Reset();
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

    public bool Update(GameUpdate update)
    => update switch {
        KeyUpdate key => KeyUpdate(key),
        _ => false
    };

    private bool KeyUpdate(KeyUpdate update)
    => KeyUpdateLock.Exclusive(() => {
        // 1) Check whether game runs:
        if (!Game.RUN_STATES.Contains(Game.State)) return false;
        // 2) Add update to be applied:
        KeyUpdates.AddLast(update);
        return true;
    });

    private void ApplyKeyUpdates(ref (GamePlayUpdate Update, bool Send) loop) {
        try {
            KeyUpdateLock.Lock(); 
            foreach (var update in KeyUpdates) {
                // 1) Update game:
                Game.Update(update);
                // 2) Add movement update to loop:
                loop.Update.Movements[update.PlayerID] = Game.NewMovementUpdate(update.PlayerID);
                // 3) Mark send:
                loop.Send = true;
            }
            KeyUpdates.Clear();
        } finally {
            KeyUpdateLock.Unlock();
        }
    }

    // Loop [Kills & Lives] ---------------------------------------------------------------------------------------------------------------
    private void EvaluateQuitPlayers(ref (GamePlayUpdate Update, bool Send) loop) {
        foreach (var (player, id) in Game.PlayerQuitIterator) {
            // 1) Kill update:
            var kill = Game.NewKillUpdate(null, id);
            Game.Update(kill);
            // 2) Movement update:
            var move = Game.NewMovementUnderMapUpdate(player);
            Game.Update(move);
            // 3) Set loop updates:
            loop.Update.Movements[id] = move;
            loop.Update.Kills[id] = kill;
            loop.Send = true;
        }
    }

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
            if (Game.Mode == GAME_MODE.MAYHEM && Game.Time < Game.ROUND_DURATION) return;
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
            if (Game.DisplayMode != DISPLAY_MODE.EACH_OWN && (loop.Update.Kills.Count > 0 || loop.Update.Lives.Count > 0)) {
                await SendGameActionUpdate(UPDATE_GROUP.ALL, loop.Update);
            // 2) Send update to all watching clients:
            } else await SendGameActionUpdate(UPDATE_GROUP.WATCH, loop.Update);
            Game.TouchClock.Reset();
        } else {
            // 3) Send periodical (wake-up) updates to touch devices:
            var deltaT = Game.TouchClock.ComputeDelta();
            if (deltaT < Game.TouchClock.INTERVAL) return;
            await SendGameUpdate(UPDATE_GROUP.WATCH_TOUCH, loop.Update);
            Game.TouchClock.Update(deltaT);
        }
    }

    // Loop -------------------------------------------------------------------------------------------------------------------------------
    private async void GameLoop() {
        try {
            // 1) Wait for the signal:
            GameStartHandle.WaitOne();
            // 2) Run loop:
            while (true) {
                // 2.1) Await delta of time:
                var deltaT = await GameLock.Exclusive(() => Game.Clock.AwaitDelta(GameLock));
                // 2.2) Update game:
                if (!await GameLock.Exclusive(async () => {
                    // 2.2.1) Quit if deleted:
                    if (IsDeleted) { ReleaseThread(); return false; }
                    // 2.2.2) Quit if finished:
                    if (Game.LOBBY_STATES.Contains(Game.State)) { await FinishRound(); return false; }
                    // 2.2.3) Pause and repeat loop:
                    if (Game.PAUSE_STATES.Contains(Game.State)) { await HandlePause(); return true; }

                    // 2.2.4) Prepare loop update & shrink:
                    var shrink = ApplyShrinking();
                    var loop = (Update: Game.NewGamePlayCurrentUpdate(), Send: shrink);
                    EvaluateQuitPlayers(ref loop);
                    EvaluateShrinking(ref loop);

                    // 2.2.5) Move game objects:
                    Game.Update(Game.NewTimeFlowUpdate(deltaT));
                    // 2.2.6) Evaluate player interactions:
                    EvaluatePlayerCollisions(ref loop);
                    // 2.2.7) Apply key updates:
                    ApplyKeyUpdates(ref loop);
                    // 2.2.8) Add life updates:
                    AddLifeUpdates(ref loop);
                    // 2.2.9) Add movement corrections:
                    EnforceMoveUpdates(ref loop, shrink);
                    // 2.2.10) Try to finish:
                    TryFinishGame(ref loop, deltaT);
                    // 2.2.11) Check action:
                    CheckActionResponse(ref loop);

                    // 2.2.12) Quit if finished:
                    if (Game.LOBBY_STATES.Contains(Game.State)) { await FinishRound(); return false; }
                    // 2.2.13) Send update & repeat loop:
                    await SendLoopUpdate(loop); return true;
                })) break;
            }
        } catch {
            // 3) Handle exception:
            await GameLock.TryExclusive(async () => {
                if (!IsDeleted) await SendException(UPDATE_GROUP.ALL, EXCEPTION.DISCONNECT);
            });
        }
    }
}
