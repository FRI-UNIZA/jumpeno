namespace Jumpeno.Shared.Models;

public class Game : IUpdateable, IRenderable<(Player? ScreenPlayer, string Font)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CODE_ID = "game-code";
    public const byte CODE_LENGTH = 4;
    
    public const string NAME_ID = "game-name";
    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 20;

    public const string CAPACITY_ID = "game-capacity";
    public const byte MIN_CAPACITY = 2;
    public const byte MAX_CAPACITY = 10;

    public const string SPECTATOR_ID = "game-spectator";
    public const int MAX_SPECTATORS = 100;

    public const float MAP_WIDTH = 1024;
    public const float MAP_HEIGHT = 576;
    public const string DEFAULT_BACKGROUND = "36, 30, 59";
    public const string DEFAULT_FOREGROUND = "255, 255, 0";
    
    public const int FPS = 60;
    public const int TOUCH_DEVICE_NOTIFICATIONS_PER_SECOND = 10;

    public const double ROUND_DURATION = 5 * 60000; // ms
    public const double ROUND_FINISH_DELAY = 3000; // ms
    
    public static readonly List<GAME_STATE> LOBBY_STATES = [GAME_STATE.LOBBY, GAME_STATE.SCOREBOARD];
    public static readonly List<GAME_STATE> PAUSE_STATES = [GAME_STATE.PAUSE];
    public static readonly List<GAME_STATE> RUN_STATES = [GAME_STATE.GAMEPLAY, GAME_STATE.SHRINKING];

    // Mockup -----------------------------------------------------------------------------------------------------------------------------
    public const string MOCK_CODE = "FRI1";
    public const string MOCK_NAME = "FRI UNIZA 1";
    public static List<Tile> MOCK_TILES() {
        List<Tile> tiles = [];
        // tiles.Add(new(new(0 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(1 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(2 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(5 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(6 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(7 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(8 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(9 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(10 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 1 * Tile.SIZE + Tile.HALF_SIZE)));
        // tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        // tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 3 * Tile.SIZE + Tile.HALF_SIZE)));
        // tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 4 * Tile.SIZE + Tile.HALF_SIZE)));
        // tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 5 * Tile.SIZE + Tile.HALF_SIZE)));
        // tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 6 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 7 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 8 * Tile.SIZE + Tile.HALF_SIZE)));
        return tiles;
    }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateCode(string code) {
        var errors = new List<Error>();
        if (code.Length != CODE_LENGTH) errors.Add(new Error(CODE_ID, "Length must be equal to I18N{length}", new() {{"length", $"{CODE_LENGTH}"}}));
        if (!Checker.IsAlphaNum(code)) errors.Add(new Error(CODE_ID, "Code must be alphanumeric"));
        if (code.ToUpper() != code) errors.Add(new Error(CODE_ID, "Code must be uppercase"));
        return errors;
    }
    public static void CheckCode(string code) {
        var errors = ValidateCode(code);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    public static List<Error> ValidateName(string name) {
        name = name.Trim();
        var errors = new List<Error>();
        if (name.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < name.Length) errors.Add(new Error(
            NAME_ID,
            "Length is not between I18N{min} and I18N{max}",
            new() {{"min", $"{NAME_MIN_LENGTH}"}, {"max", $"{NAME_MAX_LENGTH}"}}
        ));
        if (!Checker.IsAlphaNum(name, ['.', ' '])) errors.Add(new Error(NAME_ID, "Value contains not allowed character"));
        if (name.Length > 0 && name[0] == '.') errors.Add(new Error(NAME_ID, "Value must not start with a dot"));
        return errors;
    }
    public static void CheckName(string name) {
        var errors = ValidateName(name);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    public static List<Error> ValidateCapacity(byte capacity) {
        var errors = new List<Error>();
        if (capacity < MIN_CAPACITY || MAX_CAPACITY < capacity) errors.Add(
            new Error(
                CAPACITY_ID, "Capacity not between I18N{min} and I18N{max}",
                new() {{"min", $"{MIN_CAPACITY}"}, {"max", $"{MAX_CAPACITY}"}}
            )
        );
        return errors;
    }
    public static void CheckCapacity(byte capacity) {
        var errors = ValidateCapacity(capacity);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    public static List<Error> ValidateConnectionType(Connection connection) {
        var errors = new List<Error>();
        if (connection.GetType() != typeof(Connection))
            errors.Add(new Error(Connection.CONNECTION_ID, "Connection type invalid!"));
        return errors;
    }
    public static void CheckConnectionType(Connection connection) {
        var errors = ValidateConnectionType(connection);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    public static List<Error> ValidatePlayerCount(Game game) {
        var errors = new List<Error>();
        if (game.Capacity <= game.ActivePlayers.Count) errors.Add(
            new Error(CAPACITY_ID, "The game is currently full!")
        );
        return errors;
    }
    public static void CheckPlayerCount(Game game) {
        var errors = ValidatePlayerCount(game);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    public static List<Error> ValidateSpectatorCount(Game game) {
        var errors = new List<Error>();
        if (MAX_SPECTATORS <= game.SpectatorCount) errors.Add(
            new Error(SPECTATOR_ID, "Game can not have more spectators!")
        );
        return errors;
    }
    public static void CheckSpectatorCount(Game game) {
        var errors = ValidateSpectatorCount(game);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Base:
    public DISPLAY_MODE DisplayMode { get; private set; }
    public GAME_MODE Mode { get; private set;}
    public User Host { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public Map Map { get; private set; }
    public byte Capacity { get; private set; }
    // State:
    public int Round { get; private set; }
    public double Time { get; private set; }
    public GAME_STATE State { get; private set; }
    public GameClock Clock { get; private set; }
    public GameClock TouchClock { get; private set; }
    // Players:
    [JsonInclude]
    private List<Player> ActivePlayers { get; set; }
    // NOTE: Index contains all possible game players:
    [JsonInclude]
    private Dictionary<byte, Player> Players { get; set; }
    // NOTE: QuadTree of active players:
    private QuadTreeRectF<Player> PlayersQT { get; set; }
    public int AlivePlayerCount { get; private set; }
    // Spectators:
    public int SpectatorCount { get; private set; }
    // Traffic:
    public double? Ping { get; set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Game(
        DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name,
        Map map, byte capacity,
        int round, double time, GAME_STATE state,
        List<Player> activePlayers, Dictionary<byte, Player> players, int spectatorCount
    ) {
        CheckCode(code);
        CheckName(name);
        CheckCapacity(capacity);
        DisplayMode = displayMode;
        Mode = mode;
        Host = host;
        Code = code;
        Name = name.Trim();
        Map = map;
        Capacity = capacity;
        Round = round;
        Time = time;
        State = state;
        Clock = new(FPS);
        TouchClock = new(TOUCH_DEVICE_NOTIFICATIONS_PER_SECOND);
        ActivePlayers = InitActivePlayers(activePlayers, players);
        Players = InitPlayers(ActivePlayers, players);
        PlayersQT = InitPlayersQT(ActivePlayers);
        AlivePlayerCount = InitAlivePlayerCount(ActivePlayers);
        SpectatorCount = spectatorCount;
        Ping = null;
    }
    public Game(DISPLAY_MODE displayMode, GAME_MODE mode, User host, string code, string name, byte capacity) : this(
        displayMode, mode, host, code, name,
        new(0, MAP_WIDTH, 0, MAP_HEIGHT, MOCK_TILES(), DEFAULT_BACKGROUND, DEFAULT_FOREGROUND),
        capacity, 0, 0, GAME_STATE.LOBBY,
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

    private static int InitAlivePlayerCount(List<Player> activePlayers) {
        var alive = 0;
        foreach (var player in activePlayers) {
            if (player.IsAlive) alive++;
        }
        return alive;
    }

    // Player methods ---------------------------------------------------------------------------------------------------------------------
    public Player ConnectPlayer(Connection connection) {
        CheckConnectionType(connection);
        User.CheckUnknown(connection.User);
        foreach (var (id, player) in Players) {
            if (player.IsConnected && player.User.Name == connection.User.Name) {
                throw new Exception("Player name is taken!");
            }
            if (!player.IsConnected && (State == GAME_STATE.LOBBY || player.User.Equals(connection.User))) {
                player.Synchronize(connection);
                return player;
            }
        }
        if (State == GAME_STATE.LOBBY) throw new Exception("The game is currently full!");
        else throw new Exception("The game is already running.");
    }

    public Player? GetPlayerRef(byte id) {
        Players.TryGetValue(id, out var player);
        return player;
    }

    public IEnumerable<(Player player, int index)> PlayerIterator { get {
        int index = 0;
        foreach (var player in ActivePlayers) {
            yield return (player, index++);
        }
    }}

    public IEnumerable<(Player player, int index)> PlayerScoreIterator { get {
        var players = ActivePlayers.OrderByDescending(player => player.Score);
        int index = 0;
        foreach (var player in players) {
            yield return (player, index++);
        }
    }}

    public List<Player> GetCollidingPlayers(Player player) {
        return PlayersQT.GetObjects(player.Rect);
    }

    private void OnPlayerKill() => AlivePlayerCount--;
    private void OnPlayerAlive() => AlivePlayerCount++;

    private void MovePlayer(Player player) {
        PlayersQT.Move(player);
    }

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
        Update(new KillUpdate(null, player.ID));
        Update(new MovementUpdate(player.ID, position, Body.DEFAULT_DIRECTION, null, Body.DEFAULT_NORMAL));
        player.Body.Animation.UpdateDirection(new(random.NextDouble() < 0.5 ? 1 : -1, -1));
    }

    // Spectator methods ------------------------------------------------------------------------------------------------------------------
    public Spectator ConnectSpectator(Connection connection) {
        CheckConnectionType(connection);
        User.CheckUnknown(connection.User);
        CheckSpectatorCount(this);
        SpectatorCount++;
        return new(connection);
    }

    public void RemoveSpectator(Spectator spectator) {
        SpectatorCount = Math.Max(SpectatorCount - 1, 0);
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
            MovementUpdate(new MovementUpdate(
                player.ID,
                new(
                    player.Body.Position.Center.X,
                    Map.WorldMinY - (Mark.CalculateMarkPointTop(player.Body).Y - player.Body.Position.Center.Y)
                ),
                player.Body.Direction, player.Body.JumpFinishY
            ));
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
        switch (update.State) {
            case GAME_STATE.LOBBY:
                Round = 0;
                ActivePlayers.Clear();
                Players = InitPlayers(ActivePlayers);
                PlayersQT.Clear();
                AlivePlayerCount = InitAlivePlayerCount(ActivePlayers);
                SpectatorCount = 0;
                Updater.Reset();
            break;
        }
        if (State == GAME_STATE.PAUSE && update.State != GAME_STATE.PAUSE) {
            Clock.Reset();
        }
        Time = update.Time;
        State = update.State;
        return true;
    }

    private readonly UpdateGuard<RoundUpdate> RoundUpdateGuard = new();
    private bool RoundUpdate(RoundUpdate update) {
        return RoundUpdateGuard.Update(update, () => {
            if (update.StateUpdate.State == GAME_STATE.GAMEPLAY) {
                AnonymizePlayers();
                Clock.Reset();
                TouchClock.Reset();
            }

            Round = update.Round;
            StateUpdate(update.StateUpdate);
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

    // Partial update generators ----------------------------------------------------------------------------------------------------------
    public KillUpdate NewKillUpdate(byte? killerID, byte deadID) => new(killerID, deadID);
    public LifeUpdate NewLifeUpdate(Player player, Random? random = null, Dictionary<float, bool>? used = null) {
        RandomizePosition(player, random, used);
        return new(player, Body.IMMORTAL_MS);
    }
    public MovementUpdate NewMovementUpdate(byte playerID) {
        if (!Players.TryGetValue(playerID, out var player)) throw new ArgumentException("Wrong player ID!");
        return new(playerID, player.Body.Position.Center, player.Body.Direction, player.Body.JumpFinishY);
    }
    public StateUpdate NewStateUpdate(double time, GAME_STATE state) => new(time, state);
    public TimeFlowUpdate NewTimeFlowUpdate(double deltaT) => new(this, deltaT);

    // Network update generators ----------------------------------------------------------------------------------------------------------
    private readonly NetworkUpdater Updater = new();
    public GamePlayUpdate NewGamePlayUpdate(
        StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null,
        Dictionary<byte, LifeUpdate>? lives = null
    ) => Updater.NewGamePlayUpdate(Round, stateUpdate, movements, kills, lives);
    public GamePlayUpdate NewGamePlayCurrentUpdate() => NewGamePlayUpdate(NewStateUpdate(Time, State));
    public KeyUpdate NewKeyUpdate(byte playerID, LinkedList<Control> controls) => Updater.NewKeyUpdate(Round, playerID, controls);
    public PingUpdate NewPingUpdate() => Updater.NewPingUpdate(Round, DateTime.UtcNow);
    public PlayerUpdate NewPlayerUpdate(Player player) => Updater.NewPlayerUpdate(Round, player);
    public RoundUpdate NewRoundStartUpdate() {
        RestorePlayers();
        return Updater.NewRoundUpdate(Round + 1, new(0, GAME_STATE.GAMEPLAY), Players);
    }
    public RoundUpdate NewRoundFinishUpdate() => Updater.NewRoundUpdate(Round, new(Time, GAME_STATE.SCOREBOARD), Players);
    public SpectatorUpdate NewSpectatorUpdate() => Updater.NewSpectatorUpdate(Round, SpectatorCount);

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task Render(Canvas2DContext ctx, (Player? ScreenPlayer, string Font) @params) {
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
    }
}
