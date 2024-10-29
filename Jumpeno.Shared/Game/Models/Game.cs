namespace Jumpeno.Shared.Models;

public class Game {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CODE_ID = "game-code";
    public const byte CODE_LENGTH = 5;
    public const string CAPACITY_ID = "game-capacity";
    public const byte MIN_CAPACITY = 2;
    public const byte MAX_CAPACITY = 10;
    public static readonly Game EMPTY = new("EMPTY", MAX_CAPACITY);

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
        if (game.Capacity <= game.Players.Count) errors.Add(
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
    public double Time { get; private set; }
    public GAME_STATE State { get; private set; }
    public byte Capacity { get; private set; }
    [JsonInclude]
    private List<Player> Players { get; set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Game(string code, double time, GAME_STATE state, byte capacity, List<Player> players) {
        CheckCode(code);
        CheckCapacity(capacity);
        Code = code;
        Time = time;
        State = state;
        Capacity = capacity;
        Players = players;    
    }
    public Game(string code, byte capacity) : this(code, 0, GAME_STATE.LOBBY, capacity, []) {}

    // Player methods ---------------------------------------------------------------------------------------------------------------------
    public Player AddPlayer(string connectionID, User user) {
        CheckPlayerCount(this);
        var player = new Player(connectionID, user, new Avatar());
        Players.Add(player);
        return player;
    }

    public Player AddPlayer(Player player) {
        CheckPlayerCount(this);
        Players.Add(player);
        return player;
    }

    public void RemovePlayer(Player player) {
        for (int i = 0; i < Players.Count; i++) {
            if (Players[i].Equals(player)) {
                Players.RemoveAt(i);
                return;
            }
        }
    }

    public IEnumerable<Player> PlayersIterator { get {
        foreach (var player in Players) {
            yield return player;
        }
    }}

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public void Update(GameUpdate update) {
        if (update is GamePlayerUpdate u) {
            if (u.Action == PLAYER_ACTION.JOIN) AddPlayer(u.Player);
            else RemovePlayer(u.Player);
        }
    }
}
