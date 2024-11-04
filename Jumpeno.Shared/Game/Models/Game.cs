namespace Jumpeno.Shared.Models;

public class Game: IUpdateAble {
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

    // Mockup -----------------------------------------------------------------------------------------------------------------------------
    public const string MOCK_CODE = "FRI25";
    public const string MOCK_NAME = "FRI UNIZA 1";

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
    public string Code { get; private set; }
    public string Name { get; private set; }
    public double Time { get; private set; }
    public GAME_STATE State { get; private set; }
    public byte Capacity { get; private set; }
    [JsonInclude]
    private List<Player> ActivePlayers { get; set; }
    // NOTE: Index contains all possible game players:
    private Dictionary<byte, Player> Players { get; set; }
    public Map Map { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Game(string code, string name, double time, GAME_STATE state, byte capacity, List<Player> activePlayers, Map map) {
        CheckCode(code);
        CheckName(name);
        CheckCapacity(capacity);
        Code = code;
        Name = name.Trim();
        Time = time;
        State = state;
        Capacity = capacity;
        ActivePlayers = activePlayers;
        Players = InitPlayers(ActivePlayers);
        Map = map;
    }
    public Game(string code, string name, byte capacity) : this(code, name, 0, GAME_STATE.LOBBY, capacity, [], new(0, MAP_WIDTH, 0, MAP_HEIGHT)) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private Dictionary<byte, Player> InitPlayers(List<Player> activePlayers) {
        Dictionary<byte, Player> players = [];
        for (byte i = 0; i < Capacity; i++) players.Add(i, new Player(i));
        foreach (var player in activePlayers) players[player.ID] = player;
        return players;
    }

    // Player methods ---------------------------------------------------------------------------------------------------------------------
    public Player AllocatePlayer(User user) {
        bool nameTaken = false;
        foreach (var p in Players) {
            Player player = p.Value;
            if (player.Connected) {
                if (player.User != null && player.User.Name == user.Name) nameTaken = true;
            } else if (State == GAME_STATE.LOBBY || (player.User != null && player.User.Equals(user))) {
                if (nameTaken) throw new Exception(I18N.T("Player name is taken!"));
                player.ConnectUser(user);
                return player;
            }
        }
        throw new Exception(I18N.T("The game is currently full!"));
    }

    public IEnumerable<(Player player, int index)> PlayerIterator { get {
        int index = 0;
        foreach (var player in ActivePlayers) {
            yield return (player, index++);
        }
    }}

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update) {
        if (update is PlayerUpdate p) return PlayerUpdate(p);
        if (update is GamePlayUpdate g) return GamePlayUpdate(g);
        return false;
    }

    private bool PlayerUpdate(PlayerUpdate update) {
        var player = Players[update.Player.ID];
        if (!player.Update(update)) return false;
        if (update.Action == PLAYER_ACTION.JOIN) ActivePlayers.Add(player);
        else ActivePlayers.Remove(player);
        return true;
    }

    private bool GamePlayUpdate(GamePlayUpdate update) {
        Time = update.Time;
        State = update.State;
        if (update.State == GAME_STATE.LOBBY) {
            ActivePlayers.Clear();
            Players = InitPlayers(ActivePlayers);
        }
        return true;
    }
}
