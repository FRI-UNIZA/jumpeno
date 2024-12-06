namespace Jumpeno.Shared.Models;

public class Game : IUpdateable, IRenderablePure {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CODE_ID = "game-code";
    public const byte CODE_LENGTH = 5;
    public const string NAME_ID = "game-name";
    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 20;
    public const string CAPACITY_ID = "game-capacity";
    public const byte MIN_CAPACITY = 2;
    public const byte MAX_CAPACITY = 10;
    public const float MAP_WIDTH = 1024;
    public const float MAP_HEIGHT = 576;
    public const int FPS = 60;
    public const int TOUCH_DEVICE_NOTIFICATIONS_PER_SECOND = 5;
    public const string DEFAULT_BACKGROUND = "36, 30, 59";

    // Mockup -----------------------------------------------------------------------------------------------------------------------------
    public const string MOCK_CODE = "FRI25";
    public const string MOCK_NAME = "FRI UNIZA 1";
    public static List<Tile> MOCK_TILES() {
        List<Tile> tiles = [];
        tiles.Add(new(new(0 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(1 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(2 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(5 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(6 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(7 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(8 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(9 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 1 * Tile.SIZE + Tile.HALF_SIZE)));
        tiles.Add(new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)));
        return tiles;
    }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateCode(string code) {
        var errors = new List<Error>();
        if (code.Length != CODE_LENGTH) errors.Add(new(CODE_ID, I18N.T("Length must be equal to I18N{length}", new() { { "length", $"{CODE_LENGTH}" } })));
        if (!Checker.IsAlphaNum(code)) errors.Add(new(CODE_ID, I18N.T("Code must be alphanumeric")));
        if (code.ToUpper() != code) errors.Add(new(CODE_ID, I18N.T("Code must be uppercase")));
        return errors;
    }
    public static void CheckCode(string code) {
        var errors = ValidateCode(code);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    public static List<Error> ValidateName(string name) {
        name = name.Trim();
        var errors = new List<Error>();
        if (name.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < name.Length) errors.Add(new(
            NAME_ID,
            I18N.T("Length is not between I18N{min} and I18N{max}", new() { { "min", $"{NAME_MIN_LENGTH}" }, { "max", $"{NAME_MAX_LENGTH}" } })
        ));
        if (!Checker.IsAlphaNum(name, ['.', ' '])) errors.Add(new(NAME_ID, I18N.T("Value contains not allowed character")));
        if (name.Length > 0 && name[0] == '.') errors.Add(new(NAME_ID, I18N.T("Value must not start with a dot")));
        return errors;
    }
    public static void CheckName(string name) {
        var errors = ValidateName(name);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    public static List<Error> ValidateCapacity(byte capacity) {
        var errors = new List<Error>();
        if (capacity < MIN_CAPACITY || MAX_CAPACITY < capacity) errors.Add(
            new(CAPACITY_ID, I18N.T("Capacity not between I18N{min} and I18N{max}", new() { {"min", $"{MIN_CAPACITY}"}, {"max", $"{MAX_CAPACITY}"} }))
        );
        return errors;
    }
    public static void CheckCapacity(byte capacity) {
        var errors = ValidateCapacity(capacity);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    public static List<Error> ValidatePlayerCount(Game game) {
        var errors = new List<Error>();
        if (game.Capacity <= game.ActivePlayers.Count) errors.Add(
            new(CAPACITY_ID, I18N.T("The game is currently full!"))
        );
        return errors;
    }
    public static void CheckPlayerCount(Game game) {
        var errors = ValidatePlayerCount(game);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Base:
    public ACCESS_MODE Access { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public Map Map { get; private set; }
    public byte Capacity { get; private set; }
    // State:
    public double Time { get; private set; }
    public GAME_STATE State { get; private set; }
    public GameClock Clock { get; private set; }
    public GameClock TouchClock { get; private set; }
    // Players:
    [JsonInclude]
    private List<Player> ActivePlayers { get; set; }
    // NOTE: Index contains all possible game players:
    private Dictionary<byte, Player> Players { get; set; }
    // NOTE: QuadTree of active players:
    private QuadTreeRectF<Player> PlayersQT { get; set; }
    // Traffic:
    public double? Latency { get; set; }
    public double? Ping { get; set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Game(ACCESS_MODE access, string code, string name, Map map, byte capacity, double time, GAME_STATE state, List<Player> activePlayers) {
        CheckCode(code);
        CheckName(name);
        CheckCapacity(capacity);
        Access = access;
        Code = code;
        Name = name.Trim();
        Map = map;
        Capacity = capacity;
        Time = time;
        State = state;
        Clock = new(FPS);
        TouchClock = new(TOUCH_DEVICE_NOTIFICATIONS_PER_SECOND);
        ActivePlayers = activePlayers;
        Players = InitPlayers(ActivePlayers);
        PlayersQT = InitPlayersQT(ActivePlayers);
        Latency = null;
        Ping = null;
    }
    public Game(ACCESS_MODE access, string code, string name, byte capacity)
    : this(access, code, name, new(0, MAP_WIDTH, 0, MAP_HEIGHT, MOCK_TILES(), DEFAULT_BACKGROUND), capacity, 0, GAME_STATE.LOBBY, []) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private Dictionary<byte, Player> InitPlayers(List<Player> activePlayers) {
        Dictionary<byte, Player> players = [];
        for (byte i = 0; i < Capacity; i++) players.Add(i, new Player(i));
        foreach (var player in activePlayers) players[player.ID] = player;
        return players;
    }

    private QuadTreeRectF<Player> InitPlayersQT(List<Player> activePlayers) {
        QuadTreeRectF<Player> players = new(Map.Rect);
        foreach (var player in activePlayers) players.Add(player);
        return players;
    }

    // Player methods ---------------------------------------------------------------------------------------------------------------------
    public Player AllocatePlayer(User user, bool touchDevice = false) {
        bool nameTaken = false;
        foreach (var p in Players) {
            Player player = p.Value;
            if (player.Connected) {
                if (player.User != null && player.User.Name == user.Name) nameTaken = true;
            } else if (State == GAME_STATE.LOBBY || (player.User != null && player.User.Equals(user))) {
                if (nameTaken) throw new Exception(I18N.T("Player name is taken!"));
                player.ConnectUser(user, touchDevice);
                return player;
            }
        }
        if (State == GAME_STATE.LOBBY) throw new Exception(I18N.T("The game is currently full!"));
        else throw new Exception(I18N.T("The game is already running."));
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

    public List<Player> GetCollidingPlayers(Player player) {
        return PlayersQT.GetObjects(player.Rect);
    }

    private void MovePlayer(Player player) {
        PlayersQT.Move(player);
    }

    private void RestorePlayers() {
        foreach (var player in ActivePlayers) {
            player.Update(NewLifeUpdate(player.ID));
        }
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update) {
        if (update is TimeFlowUpdate time) return TimeFlowUpdate(time);
        if (update is KeyUpdate key) return KeyUpdate(key);
        if (update is GamePlayUpdate game) return GamePlayUpdate(game);
        if (update is PingUpdate ping) return PingUpdate(ping);
        if (update is KillUpdate kill) return KillUpdate(kill);
        if (update is PlayerUpdate player) return PlayerUpdate(player);
        if (update is StateUpdate state) return StateUpdate(state);
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
        Players.TryGetValue(update.PlayerID, out var player);
        if (player == null) return false;
        return player.Update(update);
    }

    readonly UpdateGuard<GamePlayUpdate> GamePlayUpdateGuard = new();
    private bool GamePlayUpdate(GamePlayUpdate update) {
        // 1) Prepare response:
        var response = update.Response = new();
        // 2) Update latency:
        Latency = GameClock.DeltaAhead(update.CreatedAt);
        // 3) Update state (sync with server):
        response.StateUpdated = GamePlayUpdateGuard.Update(update, () => {
            StateUpdate(NewStateUpdate(update.StateUpdate.Time, update.StateUpdate.State));
        });
        // 4) Apply kill updates:
        foreach (var (key, killUpdate) in update.Kills) {
            if (KillUpdate(killUpdate)) response.KillsUpdated = true;
        }
        // 5) Update player movements (sync with server):
        switch (update.StateUpdate.State) {
            case GAME_STATE.GAMEPLAY:
                foreach (var movement in update.Movements) {
                    if (Players[movement.Key].Update(update)) response.PlayersUpdated = true;
                }
            break;
        }
        // 6) Return result:
        return response.Updated;
    }

    public bool PingUpdate(PingUpdate update) {
        Ping = GameClock.DeltaAhead(update.CreatedAt);
        return true;
    }

    private bool KillUpdate(KillUpdate update) {
        var updated = Players[update.DeadID].Update(update);
        if (update.KillerID is not byte killerID) return updated;
        return Players[killerID].Update(update);
    }

    private bool PlayerUpdate(PlayerUpdate update) {
        var player = Players[update.Player.ID];
        if (!player.Update(update)) return false;
        if (update.Action == PLAYER_ACTION.JOIN) {
            ActivePlayers.Add(player);
            PlayersQT.Add(player);
            player.ResetUpdateGuards();
        } else {
            ActivePlayers.Remove(player);
            PlayersQT.Remove(player);
            KillUpdate(NewKillUpdate(null, player.ID));
            if (State == GAME_STATE.LOBBY) player.ForgetUser();
        }
        return true;
    }

    private bool StateUpdate(StateUpdate update) {
        switch (update.State) {
            case GAME_STATE.LOBBY:
                ActivePlayers.Clear();
                Players = InitPlayers(ActivePlayers);
                PlayersQT.Clear();
                Updater.Reset();
            break;
            case GAME_STATE.GAMEPLAY:
                if (State == GAME_STATE.LOBBY) {
                    Clock.Reset();
                    TouchClock.Reset();
                    RestorePlayers();
                }
            break;
        }
        Time = update.Time;
        State = update.State;
        return true;
    }

    // Partial update generators ----------------------------------------------------------------------------------------------------------
    public KillUpdate NewKillUpdate(byte? killerID, byte deadID) => new(killerID, deadID);
    public LifeUpdate NewLifeUpdate(byte playerID) => new(playerID);
    public MovementUpdate NewMovementUpdate(byte playerID, ulong? keyUpdateID = null) {
        var body = Players[playerID].Body;
        return new(playerID, keyUpdateID, body.Position.Center, body.Direction, body.JumpFinishY);
    }
    public StateUpdate NewStateUpdate(double time, GAME_STATE state) => new(time, state);
    public TimeFlowUpdate NewTimeFlowUpdate(double deltaT) => new(this, deltaT);

    // Network update generators ----------------------------------------------------------------------------------------------------------
    private readonly NetworkUpdater Updater = new();
    public GamePlayUpdate NewGamePlayUpdate(
        StateUpdate stateUpdate,
        Dictionary<byte, MovementUpdate>? movements = null,
        Dictionary<byte, KillUpdate>? kills = null
    ) => Updater.NewGamePlayUpdate(stateUpdate, movements, kills);
    public KeyUpdate NewKeyUpdate(byte playerID, LinkedList<Control> controls) => Updater.NewKeyUpdate(playerID, controls);
    public PingUpdate NewPingUpdate() => Updater.NewPingUpdate(DateTime.UtcNow);
    public PlayerUpdate NewPlayerUpdate(Player player, PLAYER_ACTION action) => Updater.NewPlayerUpdate(player, action);
    public TimerUpdate NewTimerUpdate(double time, TIMER_STATE state) => Updater.NewTimerUpdate(time, state);

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task Render(Canvas2DContext ctx) {
        await Map.Render(ctx, this);
        foreach (var player in ActivePlayers) {
            await player.Render(ctx, this);
        }
    }
}
