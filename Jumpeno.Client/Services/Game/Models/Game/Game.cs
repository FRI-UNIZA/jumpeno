namespace Jumpeno.Client.Models;

#pragma warning disable CA1822

public class Game : IUpdateable, IRenderable<(Player? ScreenPlayer, string Font)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly int FPS = AppSettings.Game.FPS;
    public static readonly int TOUCH_DEVICE_NOTIFICATIONS = AppSettings.Game.TouchDeviceNotifications.PerSecond; // per second
    // Duration:
    public static readonly double ROUND_DURATION = From.MinToMS(AppSettings.Game.Round.Minutes) - Shrink.TOTAL_DURATION; // ms
    public static readonly double ROUND_FINISH_DELAY = From.SToMS(AppSettings.Game.FinishDelay.Seconds); // ms
    // States:
    public static readonly List<GAME_STATE> LOBBY_STATES = [GAME_STATE.LOBBY, GAME_STATE.SCOREBOARD];
    public static readonly List<GAME_STATE> PAUSE_STATES = [GAME_STATE.PAUSE];
    public static readonly List<GAME_STATE> RUN_STATES = [GAME_STATE.GAMEPLAY, GAME_STATE.SHRINKING];

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Identifier:
    public ulong ID { get; }
    private static ulong IDAutoIncrement = 0;
    // Settings:
    public DISPLAY_MODE DisplayMode { get; }
    public GAME_MODE Mode { get; }
    public User Host { get;  }
    public string Code { get; }
    public string Name { get; }
    public Map Map { get; }
    public bool Anonyms { get; }
    public byte Rounds { get; }
    public byte Capacity { get; }
    // State:
    public int Round { get; private set; }
    public int ShowRound => Math.Min(LOBBY_STATES.Contains(State) ? Round + 1 : Round, Rounds);
    public double Time { get; private set; }
    public GAME_STATE State { get; private set; }
    public bool IsFinished => State == GAME_STATE.SCOREBOARD && Rounds <= Round;
    // Clock:
    public readonly GameClock Clock;
    public readonly GameClock TouchClock;
    private bool ClockAutoReset = false;
    public void ClockAutoResetOn() => ClockAutoReset = true;
    public void ClockAutoResetOff() => ClockAutoReset = false;
    private void ResetClocks() { Clock.Reset(); TouchClock.Reset(); }
    // Host:
    public bool HostConnected { get; private set; }
    // Players:
    // NOTE: Index containing all possible game players:
    [JsonInclude] private Dictionary<byte, Player> Players { get; }
    // NOTE: Active players are connected:
    [JsonInclude] private List<Player> ActivePlayers { get; }
    public int ActivePlayersCount => ActivePlayers.Count;
    public int AlivePlayersCount { get; private set; }
    // NOTE: QuadTree of active players:
    private QuadTreeRectF<Player> PlayersQT { get; }
    // Spectators:
    public int SpectatorCount { get; private set; }
    // Empty:
    public bool IsEmpty => ActivePlayersCount <= 0 && SpectatorCount <= 0;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Game(
        ulong id,
        DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name,
        Map map, bool anonyms, byte rounds, byte capacity,
        int round, double time, GAME_STATE state,
        bool hostConnected,
        Dictionary<byte, Player> players, List<Player> activePlayers,
        int spectatorCount
    ) {
        // Validation:
        GameValidator.AssertCode(code);
        GameValidator.AssertName(name);
        GameValidator.AssertCapacity(capacity);
        // Identifier:
        ID = id;
        // Settings:
        DisplayMode = displayMode;
        Mode = mode;
        Host = host;
        Code = code;
        Name = name.Trim();
        Map = map;
        Anonyms = anonyms;
        Rounds = rounds;
        Capacity = capacity;
        // State:
        Round = round;
        Time = time;
        State = state;
        // Clocks:
        Clock = new(FPS);
        TouchClock = new(TOUCH_DEVICE_NOTIFICATIONS);
        // Host:
        HostConnected = hostConnected;
        // Players:
        ActivePlayers = InitActivePlayers(activePlayers, players);
        AlivePlayersCount = InitAlivePlayerCount(ActivePlayers);
        Players = InitPlayers(ActivePlayers, players);
        PlayersQT = InitPlayersQT(ActivePlayers);
        // Spectators:
        SpectatorCount = spectatorCount;
    }
    public Game(
        DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name,
        Map map, bool anonyms, byte rounds, byte capacity
    ) : this(
        IDAutoIncrement++,
        displayMode, mode, host, code, name, map, anonyms, rounds, capacity,
        0, 0, GAME_STATE.LOBBY,
        false, [], [], 0
    ) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private List<Player> InitActivePlayers(List<Player> activePlayers, Dictionary<byte, Player> players) {
        List<Player> result = [];
        foreach (var value in activePlayers) {
            if (!players.TryGetValue(value.ID, out var player)) continue;
            result.Add(player);
        }
        return result;
    }

    private static int InitAlivePlayerCount(List<Player> activePlayers) {
        var alive = 0;
        foreach (var player in activePlayers) {
            if (player.IsAlive) alive++;
        }
        return alive;
    }

    private Dictionary<byte, Player> InitPlayers(List<Player> activePlayers, Dictionary<byte, Player>? players = null) {
        if (players != null && players.Count == Capacity) return players;
        Dictionary<byte, Player> result = [];
        for (byte i = 0; i < Capacity; i++) result.Add(i, new Player(i));
        foreach (var player in activePlayers) result[player.ID] = player;
        return result;
    }

    private QuadTreeRectF<Player> InitPlayersQT(List<Player> activePlayers) {
        var padding = 2 * Animation.MAX_HEIGHT + Mark.HEIGHT;
        QuadTreeRectF<Player> players = new(
            Map.Rect.Left - padding, Map.Rect.Top - padding,
            Map.Rect.Width + 2 * padding, Map.Rect.Height + 2 * padding
        );
        foreach (var player in activePlayers) players.Add(player);
        return players;
    }

    // Player methods ---------------------------------------------------------------------------------------------------------------------
    public Player CreatePlayer(
        // Parameters:
        Connection connection,
        // Exceptions:
        string nameID = ""
    ) {
        // 1) Validation:
        AppEnvironment.AssertServer();
        UserValidator.AssertConnectionType(connection);
        UserValidator.AssertUnknown(connection.User, nameID);
        GameValidator.AssertAllowedAnonyms(this, connection.User);
        // 2) Check host:
        GameValidator.AssertPlayerHostPresentation(this, connection.User);
        GameValidator.AssertReservedPlayerHostSpace(this, connection.User);
        // 3) Find space:
        var player = FindPlayerSpace(connection, nameID);
        // 4) Set skin of anonym:
        SetSkinOfAnonymousUser(connection.User);
        // 5) Synchronize:
        player.Synchronize(connection);
        // 6) Return player:
        return new(player);
    }

    private Player FindPlayerSpace(
        // Parameters:
        Connection connection,
        // Exceptions:
        string nameID = ""
    ) {
        // 1) Try to find space:
        Player? space = null;
        foreach (var (id, player) in Players) {
            if (player.IsConnected) {
                if (player.User.Name != connection.User.Name) continue;
                if (State == GAME_STATE.LOBBY) {
                    throw EXCEPTION.DEFAULT.SetInfo("Player name is taken!")
                    .SetErrors(ERROR.DEFAULT.SetID(nameID).SetInfo("Player name is taken!"));
                } else {
                    throw EXCEPTION.DEFAULT.SetInfo("The game is already running.");
                }
            } else if (State == GAME_STATE.LOBBY || player.User.Equals(connection.User)) {
                GameValidator.AssertReservedPlayerHostName(this, connection.User, userNameID: nameID);
                space = player;
                break;
            }
        }
        // 2.1) Space not found:
        if (space == null) {
            if (State == GAME_STATE.LOBBY) throw EXCEPTION.DEFAULT.SetInfo("The game is currently full!");
            else throw EXCEPTION.DEFAULT.SetInfo("The game is already running.");
        }
        // 2.2) Or return found space:
        else return space;
    }

    private static void SetSkinOfAnonymousUser(User user) {
        // 1) Check anonym:
        if (user.ID != null) return;
        // 2) Select skin:
        user.Skin = User.GenerateSkin();
    }

    public Player? GetActivePlayer(string connectionID) {
        foreach (var player in ActivePlayers) {
            if (player.ConnectionID == connectionID) return player;
        }
        return null;
    }

    public Player? GetPlayer(byte id) {
        Players.TryGetValue(id, out var player); return player;
    }

    public List<Player> GetCollidingPlayers(Player player) => PlayersQT.GetObjects(player.Rect);

    public IEnumerable<(Player player, int index)> PlayerIterator { get {
        int index = 0;
        foreach (var player in ActivePlayers) {
            yield return (player, index++);
        }
    }}

    public IEnumerable<(Player player, int index)> PlayerScoreIterator { get {
        var players = State == GAME_STATE.LOBBY ? ActivePlayers
        : Players.Values.Where(p => !p.User.Equals(User.UNKNOWN)).ToList();
        int index = 0;
        foreach (var player in players.OrderByDescending(p => p.Score)) {
            yield return (player, index++);
        }
    }}

    public IEnumerable<(Player player, byte id)> PlayerQuitIterator { get {
        foreach (var (id, player) in Players) {
            if (!player.IsConnected && (player.Body.Alive || !player.Body.Fallen))
                yield return (player, id);
        }
    }}

    private void OnPlayerKill() => AlivePlayersCount--;
    private void OnPlayerAlive() => AlivePlayersCount++;

    private void MovePlayer(Player player) => PlayersQT.Move(player);

    private void ResurrectPlayers() {
        var rand = new Random();
        var used = new Dictionary<float, bool>();
        foreach (var player in ActivePlayers) {
            Update(NewLifeUpdate(player, rand, used));
        }
    }

    private void KillDisconnectedPlayers() {
        foreach (var (id, player) in Players) {
            if (player.IsConnected) continue;
            Update(NewKillUpdate(null, id));
            Update(NewMovementUnderMapUpdate(player));
        }
    }

    private void RandomizePosition(Player player, Random? random = null, Dictionary<float, bool>? used = null) {
        // 1) Prepare parameters:
        random ??= new Random();
        used ??= [];
        // 2) Randomize position:
        float x = Map.WorldMinX + random.Next(0, (int) Map.WorldWidth) / Tile.SIZE * Tile.SIZE + Tile.HALF_SIZE;
        while (used.ContainsKey(x)) x = Map.WorldMinX + (x - Map.WorldMinX + Tile.SIZE) % Map.WorldWidth;
        used[x] = true;
        var y = Map.WorldMinY + random.Next(0, (int) Map.WorldHeight) / Tile.SIZE * Tile.SIZE + Body.HALF_HEIGHT;
        // 3) Avoid tile collision:
        var position = new PointF(x, y);
        while (Map.GetCollidingTiles(new(position.X - Tile.HALF_SIZE, position.Y - Tile.HALF_SIZE, Tile.SIZE, Tile.SIZE)).Count > 0) {
            position.Y = Map.WorldMinY + (position.Y - Map.WorldMinY + Tile.SIZE) % Map.WorldHeight;
        }
        // 4) Put on the ground:
        while (
            Map.GetCollidingTiles(new(position.X - Tile.HALF_SIZE, position.Y - Tile.HALF_SIZE, Tile.SIZE, Tile.SIZE)).Count <= 0
            && (position.Y > Map.WorldMinY)
        ) position.Y -= Tile.SIZE;
        position.Y += Tile.SIZE;
        // 5) Update player:
        Update(NewKillUpdate(null, player.ID));
        Update(new MovementUpdate(player.ID, position, Body.DEFAULT_DIRECTION, null, Body.DEFAULT_NORMAL));
        player.Body.Animation.ResetDirection(new(random.NextDouble() < 0.5 ? 1 : -1, -1));
    }

    // Spectator methods ------------------------------------------------------------------------------------------------------------------
    public Spectator CreateSpectator(
        // Parameters:
        Connection connection,
        // Exceptions:
        string nameID = ""
    ) {
        // 1) Validation:
        AppEnvironment.AssertServer();
        UserValidator.AssertConnectionType(connection);
        UserValidator.AssertUnknown(connection.User, nameID);
        GameValidator.AssertSpectatorCount(this);
        // 2) Check host:
        GameValidator.AssertSpectatorHostNonPresentation(this, connection.User);
        GameValidator.AssertHostAlreadyConnected(this, connection.User);
        GameValidator.AssertReservedSpectatorHostSpace(this, connection.User);
        // 3) Return spectator:
        return new(connection);
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update)
    => update switch {
        TimeFlowUpdate time => TimeFlowUpdate(time),
        KeyUpdate key => KeyUpdate(key),
        GamePlayUpdate game => GamePlayUpdate(game),
        MovementUpdate move => MovementUpdate(move),
        KillUpdate kill => KillUpdate(kill),
        LifeUpdate life => LifeUpdate(life),
        PlayerUpdate player => PlayerUpdate(player),
        SpectatorUpdate watch => SpectatorUpdate(watch),
        StateUpdate state => StateUpdate(state),
        RoundUpdate round => RoundUpdate(round),
        _ => false
    };

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        if (update.DeltaT <= 0) return false;
        Time += update.DeltaT;
        Map.Update(update);
        foreach (var (_, player) in Players) {
            player.Update(update);
        }
        foreach (var player in ActivePlayers) {
            MovePlayer(player);
        }
        return true;
    }

    private bool KeyUpdate(KeyUpdate update) {
        // 1) Check correct state:
        if (update.Round != Round) return false;
        if (!RUN_STATES.Contains(State)) return false;
        // 2) Find player:
        Players.TryGetValue(update.PlayerID, out var player);
        if (player == null) return false;
        // 3) Execute update:
        return player.Update(update);
    }

    readonly UpdateGuard<GamePlayUpdate> GamePlayUpdateGuard = new();
    private bool GamePlayUpdate(GamePlayUpdate update) {
        // 1) Check correct state:
        if (update.Round != Round) return false;
        if (LOBBY_STATES.Contains(State)) return false;
        // 2) Prepare response with state update:
        var response = new GamePlayResponse {
            StateUpdated = GamePlayUpdateGuard.Update(update, () => StateUpdate(update.StateUpdate))
        };
        // 3) Prepare updates:
        HashSet<byte> updates = [];
        Dictionary<byte, KillUpdate> scoreUpdates = [];
        foreach (var id in update.Movements.Keys) updates.Add(id);
        foreach (var id in update.Kills.Keys) updates.Add(id);
        foreach (var id in update.Lives.Keys) updates.Add(id);
        foreach (var (id, killUpdate) in update.Kills) {
            if (killUpdate.KillerID is not byte killerID || id == killUpdate.KillerID) continue;
            scoreUpdates[killerID] = killUpdate;
        }
        // 4) Apply player updates:
        foreach (var id in updates) {
            if (!Players.TryGetValue(id, out var player)) continue;
            if (player.Update(update)) {
                response.MoveUpdated = response.MoveUpdated || update.Response.MoveUpdated;
                response.KillUpdated = response.KillUpdated || update.Response.KillUpdated;
                response.LifeUpdated = response.LifeUpdated || update.Response.LifeUpdated;
                if (update.Response.KillUpdated) OnPlayerKill();
                if (update.Response.LifeUpdated) OnPlayerAlive();
            }
        }
        // 5) Update score:
        foreach (var (id, killUpdate) in scoreUpdates) {
            if (!Players.TryGetValue(id, out var player)) continue;
            if (player.Update(killUpdate)) response.ScoreUpdated = true;
        }
        // 6) Return result:
        update.Response = response;
        return response.Updated;
    }

    private bool MovementUpdate(MovementUpdate update) {
        if (!Players.TryGetValue(update.PlayerID, out var player)) return false;
        var updated = player.Update(update);
        if (updated) MovePlayer(player);
        return updated;
    }

    private bool KillUpdate(KillUpdate update) {
        // 1) Kill player:
        if (!Players.TryGetValue(update.DeadID, out var dead)) return false;
        var updated = dead.Update(update);
        if (updated) OnPlayerKill();
        // 2) Update score:
        if (update.KillerID is not byte killerID) return updated;
        if (!Players.TryGetValue(killerID, out var killer)) return updated;
        return killer.Update(update);
    }

    private bool LifeUpdate(LifeUpdate update) {
        if (!Players.TryGetValue(update.Player.ID, out var player)) return false;
        var updated = player.Update(update);
        if (updated) OnPlayerAlive();
        return updated;
    }

    private bool PlayerUpdate(PlayerUpdate update) {
        if (!Players.TryGetValue(update.Player.ID, out var player)) return false;
        if (!player.Update(update)) return false;
        // 1) Update host:
        HostConnected = update.HostConnected;
        // 2) Remove player:
        ActivePlayers.Remove(player);
        PlayersQT.Remove(player);
        // 3) Add player if connected:
        if (update.Player.IsConnected) {
            ActivePlayers.Add(player);
            PlayersQT.Add(player);
            if (AppEnvironment.IsServer) player.ResetUpdateGuards();
        }
        return true;
    }

    private readonly UpdateGuard<SpectatorUpdate> SpectatorUpdateGuard = new();
    private bool SpectatorUpdate(SpectatorUpdate update)
    => SpectatorUpdateGuard.Update(update, () => {
        HostConnected = update.HostConnected;
        SpectatorCount = update.SpectatorCount;
    });

    private bool StateUpdate(StateUpdate update) {
        // 1) Current state:
        switch (State) {
            case GAME_STATE.PAUSE:
                if (update.State != GAME_STATE.PAUSE) ResetClocks();
            break;
        }
        // 2) New state:
        switch (update.State) {
            case GAME_STATE.PAUSE:
                foreach (var player in Players.Values) {
                    player.Update(update);
                }
            break;
        }
        // 3) Update state:
        Time = update.Time;
        State = update.State;
        Map.Update(update);
        // 4) AutoReset:
        if (ClockAutoReset) ResetClocks();
        // 5) Return result:
        return true;
    }

    private readonly UpdateGuard<RoundUpdate> RoundUpdateGuard = new();
    private bool RoundUpdate(RoundUpdate update) 
    => RoundUpdateGuard.Update(update, () => {
        // 1) Reset clocks:
        ResetClocks();
        // 2) Update round & state:
        Round = update.Round;
        StateUpdate(update.StateUpdate);
        // 3) Update players (position & score):
        foreach (var (id, player) in Players) {
            player.Update(update);
        }
        // 4) Return result:
        return true;
    });

    public void ResetUpdateGuards() {
        GamePlayUpdateGuard.Reset();
        SpectatorUpdateGuard.Reset();
        RoundUpdateGuard.Reset();
    }

    // Partial update factory -------------------------------------------------------------------------------------------------------------
    public KillUpdate NewKillUpdate(byte? killerID, byte deadID, bool penalize = false) {
        return new(killerID, deadID, penalize);
    }
    public LifeUpdate NewLifeUpdate(Player player, Random? random = null, Dictionary<float, bool>? used = null) {
        RandomizePosition(player, random, used);
        return new(player, Body.IMMORTAL_MS);
    }

    public MovementUpdate NewMovementUpdate(byte playerID) {
        if (!Players.TryGetValue(playerID, out var player)) throw new ArgumentException("Wrong player ID!");
        return new(playerID, player.Body.Position.Center, player.Body.Direction, player.Body.JumpFinishY);
    }
    public MovementUpdate NewMovementUpdate(Player jumper, Player victim) {
        return new MovementUpdate(
            jumper.ID,
            new(jumper.Body.Position.Center.X, victim.Body.Position.Center.Y + Body.HEIGHT),
            jumper.Body.Direction, jumper.Body.JumpFinishY
        );
    }
    public MovementUpdate NewMovementUnderMapUpdate(Player player) {
        return new MovementUpdate(
            player.ID,
            new(
                player.Body.Position.Center.X,
                Map.WorldMinY - (Mark.CalculateMarkPointTop(player.Body).Y - player.Body.Position.Center.Y)
            ),
            player.Body.Direction, player.Body.JumpFinishY
        );
    }

    public StateUpdate NewStateUpdate(double time, GAME_STATE state, int? level = null, double? timer = null) {
        return new(time, state, level ?? Map.Shrink.Level, timer ?? Map.Shrink.Timer);
    }
    public TimeFlowUpdate NewTimeFlowUpdate(double deltaT) => new(this, deltaT);

    // Network update factory -------------------------------------------------------------------------------------------------------------
    private readonly NetworkUpdater Updater = new();

    public GamePlayUpdate NewGamePlayUpdate(
        StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null,
        Dictionary<byte, LifeUpdate>? lives = null
    ) {
        return Updater.NewGamePlayUpdate(Round, stateUpdate, movements, kills, lives);
    }
    public GamePlayUpdate NewGamePlayCurrentUpdate() {
        return NewGamePlayUpdate(NewStateUpdate(Time, State));
    }

    public KeyUpdate NewKeyUpdate(byte playerID, LinkedList<Control> controls) {
        return Updater.NewKeyUpdate(Round, playerID, controls);
    }

    public PlayerUpdate NewPlayerAddUpdate(Player player) {
        return Updater.NewPlayerUpdate(Round, player.User.ID == Host.ID || HostConnected, player, false);
    }
    public PlayerUpdate NewPlayerRemoveUpdate(Player player) {
        return Updater.NewPlayerUpdate(Round, player.User.ID != Host.ID && HostConnected, player, State == GAME_STATE.LOBBY);
    }

    public SpectatorUpdate NewSpectatorAddUpdate(Spectator spectator) {
        return Updater.NewSpectatorUpdate(Round, spectator.User.ID == Host.ID || HostConnected, SpectatorCount + 1);
    }
    public SpectatorUpdate NewSpectatorRemoveUpdate(Spectator spectator) {
        return Updater.NewSpectatorUpdate(Round, spectator.User.ID != Host.ID && HostConnected, SpectatorCount - 1);
    }

    public RoundUpdate NewRoundStartUpdate() {
        ResurrectPlayers();
        KillDisconnectedPlayers();
        return Updater.NewRoundUpdate(
            Round + 1, NewStateUpdate(0, GAME_STATE.GAMEPLAY, Shrink.DEFAULT.LEVEL, Shrink.DEFAULT.TIMER),
            Players
        );
    }
    public RoundUpdate NewRoundFinishUpdate() {
        return Updater.NewRoundUpdate(Round, NewStateUpdate(Time, GAME_STATE.SCOREBOARD), Players);
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, (Player? ScreenPlayer, string Font) @params) {
        var (screenPlayer, font) = @params;
        // 1) Render map:
        await Map.Render(ctx, this);
        // 2) Render players:
        Player? myPlayer = null;
        foreach (var player in ActivePlayers) {
            if (player.Equals(screenPlayer)) {
                myPlayer = player; continue;
            }
            await player.Render(ctx, this);
        }
        // 3) Render screen player on top:
        if (myPlayer != null) await myPlayer.Render(ctx, this);
        // 4) Render names or mark:
        foreach (var player in ActivePlayers) {
            if (player.Equals(screenPlayer)) await Mark.RenderMark(ctx, (this, player));
            else await Mark.RenderName(ctx, (this, player, font));
        }
        return true;
    }
}
