namespace Jumpeno.Shared.Models;

public class Game : IUpdateable, IRenderable<(Player? ScreenPlayer, string Font)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string DEFAULT_CODE = "FRI1";
    public const string DEFAULT_NAME = "FRI UNIZA 1";
    
    public static readonly int FPS = AppSettings.Game.FPS;
    public static readonly int TOUCH_DEVICE_NOTIFICATIONS = AppSettings.Game.TouchDeviceNotifications.PerSecond; // per second

    public static readonly double ROUND_DURATION = From.MinToMS(AppSettings.Game.Round.Minutes) - Shrink.TOTAL_DURATION; // ms
    public static readonly double ROUND_FINISH_DELAY = From.SToMS(AppSettings.Game.FinishDelay.Seconds); // ms

    public static readonly List<GAME_STATE> LOBBY_STATES = [GAME_STATE.LOBBY, GAME_STATE.SCOREBOARD];
    public static readonly List<GAME_STATE> PAUSE_STATES = [GAME_STATE.PAUSE];
    public static readonly List<GAME_STATE> RUN_STATES = [GAME_STATE.GAMEPLAY, GAME_STATE.SHRINKING];

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Settings:
    public DISPLAY_MODE DisplayMode { get; private set; }
    public GAME_MODE Mode { get; private set; }
    public User Host { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public Map Map { get; private set; }
    public byte Capacity { get; private set; }
    // State:
    public int Round { get; private set; }
    public double Time { get; private set; }
    public GAME_STATE State { get; private set; }
    // Clock:
    public GameClock Clock { get; private set; }
    public GameClock TouchClock { get; private set; }
    private void ResetClocks() { Clock.Reset(); TouchClock.Reset(); }
    // Players:
    // NOTE: Index containing all possible game players:
    [JsonInclude] private Dictionary<byte, Player> Players { get; set; }
    // NOTE: Active players are connected:
    [JsonInclude] private List<Player> ActivePlayers { get; set; }
    public int ActivePlayersCount => ActivePlayers.Count;
    public int AlivePlayersCount { get; private set; }
    // NOTE: QuadTree of active players:
    private QuadTreeRectF<Player> PlayersQT { get; set; }
    // Spectators:
    public int SpectatorCount { get; private set; }
    // Traffic:
    public double? Ping { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Game(
        DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name,
        Map map, byte capacity,
        int round, double time, GAME_STATE state,
        Dictionary<byte, Player> players, List<Player> activePlayers, int spectatorCount
    ) {
        // Validation:
        GameValidator.CheckCode(code);
        GameValidator.CheckName(name);
        GameValidator.CheckCapacity(capacity);
        // Settings:
        DisplayMode = displayMode;
        Mode = mode;
        Host = host;
        Code = code;
        Name = name.Trim();
        Map = map;
        Capacity = capacity;
        // State:
        Round = round;
        Time = time;
        State = state;
        // Clocks:
        Clock = new(FPS);
        TouchClock = new(TOUCH_DEVICE_NOTIFICATIONS);
        // Players:
        ActivePlayers = InitActivePlayers(activePlayers, players);
        AlivePlayersCount = InitAlivePlayerCount(ActivePlayers);
        Players = InitPlayers(ActivePlayers, players);
        PlayersQT = InitPlayersQT(ActivePlayers);
        // Spectators:
        SpectatorCount = spectatorCount;
        // Traffic:
        Ping = null;
    }
    public Game(DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name, Map map, byte capacity) : this(
        displayMode, mode, host, code, name, map, capacity,
        0, 0, GAME_STATE.LOBBY,
        [], [], 0
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
    public Player ConnectPlayer(Connection connection) {
        AppEnvironment.CheckServer();
        GameValidator.CheckConnectionType(connection);
        UserValidator.CheckUnknown(connection.User);
        foreach (var (id, player) in Players) {
            if (player.IsConnected) {
                if (player.User.Name != connection.User.Name) continue;
                if (State == GAME_STATE.LOBBY) throw new CoreException(new Error("Player name is taken!"));
                else throw new CoreException(new Error("The game is already running."));
            } else if (State == GAME_STATE.LOBBY || player.User.Equals(connection.User)) {
                player.Synchronize(connection);
                return player;
            }
        }
        if (State == GAME_STATE.LOBBY) throw new CoreException(new Error("The game is currently full!"));
        else throw new CoreException(new Error("The game is already running."));
    }

    public Player? GetPlayerRef(byte id) {
        Players.TryGetValue(id, out var player); return player;
    }

    public List<Player> GetCollidingPlayers(Player player) {
        return PlayersQT.GetObjects(player.Rect);
    }

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

    private void OnPlayerKill() => AlivePlayersCount--;
    private void OnPlayerAlive() => AlivePlayersCount++;

    private void MovePlayer(Player player) => PlayersQT.Move(player);

    private void RestorePlayers() {
        var rand = new Random();
        var used = new Dictionary<float, bool>();
        foreach (var player in ActivePlayers) {
            Update(NewLifeUpdate(player, rand, used));
        }
    }

    private void AnonymizePlayers() {
        foreach (var (id, player) in Players) {
            if (player.IsConnected) continue;
            Players[id] = new Player(id);
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
        player.Body.Animation.UpdateDirection(new(random.NextDouble() < 0.5 ? 1 : -1, -1));
    }

    // Spectator methods ------------------------------------------------------------------------------------------------------------------
    public Spectator ConnectSpectator(Connection connection) {
        AppEnvironment.CheckServer();
        GameValidator.CheckConnectionType(connection);
        UserValidator.CheckUnknown(connection.User);
        GameValidator.CheckSpectatorCount(this);
        SpectatorCount++;
        return new(connection);
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update) {
        if (update is TimeFlowUpdate time) return TimeFlowUpdate(time);
        if (update is KeyUpdate key) return KeyUpdate(key);
        if (update is GamePlayUpdate game) return GamePlayUpdate(game);
        if (update is PingUpdate ping) return PingUpdate(ping);
        if (update is MovementUpdate move) return MovementUpdate(move);
        if (update is KillUpdate kill) return KillUpdate(kill);
        if (update is LifeUpdate life) return LifeUpdate(life);
        if (update is PlayerUpdate player) return PlayerUpdate(player);
        if (update is SpectatorUpdate watch) return SpectatorUpdate(watch);
        if (update is StateUpdate state) return StateUpdate(state);
        if (update is RoundUpdate round) return RoundUpdate(round);
        return false;
    }

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        if (update.DeltaT <= 0) return false;
        Time += update.DeltaT;
        Map.Update(update);
        foreach (var player in ActivePlayers) {
            player.Update(update);
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
        Dictionary<byte, bool> updates = [];
        Dictionary<byte, KillUpdate> scoreUpdates = [];
        foreach (var id in update.Movements.Keys) updates[id] = true;
        foreach (var id in update.Kills.Keys) updates[id] = true;
        foreach (var id in update.Lives.Keys) updates[id] = true;
        foreach (var (id, killUpdate) in update.Kills) {
            if (killUpdate.KillerID is not byte killerID || id == killUpdate.KillerID) continue;
            scoreUpdates[killerID] = killUpdate;
        }
        // 4) Apply player updates:
        foreach (var id in updates.Keys) {
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

    private bool PingUpdate(PingUpdate update) {
        if (update.ReturnedAt is not DateTime returnedAt) return false;
        Ping = GameClock.Delta(returnedAt, update.CreatedAt);
        return true;
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
        if (update.Player.IsConnected) {
            ActivePlayers.Add(player);
            PlayersQT.Add(player);
            player.ResetUpdateGuards();
        } else {
            KillUpdate(NewKillUpdate(null, player.ID));
            MovementUpdate(NewMovementUnderMapUpdate(player));
            ActivePlayers.Remove(player);
            PlayersQT.Remove(player);
        }
        return true;
    }

    private readonly UpdateGuard<SpectatorUpdate> SpectatorUpdateGuard = new();
    private bool SpectatorUpdate(SpectatorUpdate update) {
        return SpectatorUpdateGuard.Update(update, () => {
            SpectatorCount = update.SpectatorCount;
        });
    }

    private bool StateUpdate(StateUpdate update) {
        // 1) Current state:
        switch (State) {
            case GAME_STATE.PAUSE:
                if (update.State != GAME_STATE.PAUSE) ResetClocks();
            break;
        }
        // 2) New state:
        switch (update.State) {
            // NOTE: Lobby means reset (initial state):
            case GAME_STATE.LOBBY:
                Round = 0;
                ResetClocks();
                ActivePlayers.Clear();
                AlivePlayersCount = InitAlivePlayerCount(ActivePlayers);
                Players = InitPlayers(ActivePlayers);
                PlayersQT.Clear();
                SpectatorCount = 0;
                Ping = null;
                ResetUpdateGuards();
                Updater.Reset();
            break;
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
        return true;
    }

    private readonly UpdateGuard<RoundUpdate> RoundUpdateGuard = new();
    private bool RoundUpdate(RoundUpdate update) {
        return RoundUpdateGuard.Update(update, () => {
            // 1) New state:
            switch (update.StateUpdate.State) {
                case GAME_STATE.GAMEPLAY:
                    if (update.Round == 1) AnonymizePlayers();
                    ResetClocks();
                break;
            }
            // 2) Update round & state:
            Round = update.Round;
            StateUpdate(update.StateUpdate);
            // 3) Update players (position & score):
            foreach (var (id, player) in Players) {
                player.Update(update);
            }
            return true;
        });
    }

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
            new(jumper.Body.Position.Center.X, victim.Body.Position.Center.Y + 2 * Body.HALF_HEIGHT),
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
    public TimeFlowUpdate NewTimeFlowUpdate(double deltaT) {
        return new(this, deltaT);
    }

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
    public PingUpdate NewPingUpdate() {
        return Updater.NewPingUpdate(Round, DateTime.UtcNow);
    }

    public PlayerUpdate NewPlayerUpdate(Player player) {
        return Updater.NewPlayerUpdate(Round, player);
    }
    public SpectatorUpdate NewSpectatorUpdate() {
        return Updater.NewSpectatorUpdate(Round, SpectatorCount);
    }
    public SpectatorUpdate NewSpectatorRemoveUpdate() {
        return Updater.NewSpectatorUpdate(Round, Math.Max(SpectatorCount - 1, 0));
    }

    public RoundUpdate NewRoundResetUpdate() {
        return Updater.NewRoundUpdate(0, NewStateUpdate(0, GAME_STATE.LOBBY, Shrink.DEFAULT.LEVEL, Shrink.DEFAULT.TIMER), []);
    }
    public RoundUpdate NewRoundStartUpdate() {
        RestorePlayers();
        return Updater.NewRoundUpdate(Round + 1, NewStateUpdate(0, GAME_STATE.GAMEPLAY, Shrink.DEFAULT.LEVEL, Shrink.DEFAULT.TIMER), Players);
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
        foreach (var player in ActivePlayers) {
            if (player.Equals(screenPlayer)) continue;
            await player.Render(ctx, this);
        }
        // 3) Render screen player on top:
        if (screenPlayer != null) await screenPlayer.Render(ctx, this);
        // 4) Render names or mark:
        foreach (var player in ActivePlayers) {
            if (player.Equals(screenPlayer)) await Mark.RenderMark(ctx, (this, player));
            else await Mark.RenderName(ctx, (this, player, font));
        }
        return true;
    }
}
