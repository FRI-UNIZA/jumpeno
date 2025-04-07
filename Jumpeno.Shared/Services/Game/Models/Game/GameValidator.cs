namespace Jumpeno.Shared.Utils;

public static class GameValidator {
    // Code -------------------------------------------------------------------------------------------------------------------------------
    public const string CODE = "Game.Code";

    // Constants:
    public const byte CODE_LENGTH = 4;

    public static List<Error> ValidateCode(string code) {
        var errors = Checker.Validate(code.Length != CODE_LENGTH,
            new Error(CODE, "Length must be equal to I18N{length}", new() {{"length", $"{CODE_LENGTH}"}})
        );
        Checker.Validate(errors, !Checker.IsAlphaNum(code), new Error(CODE, "Code must be alphanumeric"));
        Checker.Validate(errors, code.ToUpper() != code, new Error(CODE, "Code must be uppercase"));
        return errors;
    }
    public static void CheckCode(string code) => Checker.Check(ValidateCode(code));

    // Name -------------------------------------------------------------------------------------------------------------------------------
    public const string NAME = "Game.Name";
    
    // Constants:
    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 20;

    public static List<Error> ValidateName(string name) {
        name = name.Trim();
        var errors = Checker.Validate(
            name.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < name.Length,
            new Error(
                NAME,
                "Length is not between I18N{min} and I18N{max}",
                new() {{"min", $"{NAME_MIN_LENGTH}"}, {"max", $"{NAME_MAX_LENGTH}"}}
            )
        );
        Checker.Validate(errors, !Checker.IsAlphaNum(name, ['.', ' ']), new Error(NAME, "Value contains not allowed character"));
        Checker.Validate(errors, name.Length > 0 && name[0] == '.', new Error(NAME, "Value must not start with a dot"));
        return errors;
    }
    public static void CheckName(string name) => Checker.Check(ValidateName(name));

    // Capacity ---------------------------------------------------------------------------------------------------------------------------
    public const string CAPACITY = "Game.Capacity";

    // Constants:
    public const byte MIN_CAPACITY = 2;
    public const byte MAX_CAPACITY = 10;

    public static List<Error> ValidateCapacity(byte capacity) => Checker.Validate(
        capacity < MIN_CAPACITY || MAX_CAPACITY < capacity,
        new Error(
            CAPACITY, "Capacity not between I18N{min} and I18N{max}",
            new() {{"min", $"{MIN_CAPACITY}"}, {"max", $"{MAX_CAPACITY}"}}
        )
    );
    public static void CheckCapacity(byte capacity) => Checker.Check(ValidateCapacity(capacity));

    public static List<Error> ValidatePlayerCount(Game game) => Checker.Validate(
        game.Capacity <= game.ActivePlayersCount,
        new Error(CAPACITY, "The game is currently full!")
    );
    public static void CheckPlayerCount(Game game) => Checker.Check(ValidatePlayerCount(game));

    // Connection -------------------------------------------------------------------------------------------------------------------------
    public const string CONNECTION = "Game.Connection";

    public static List<Error> ValidateConnectionType(Connection connection) => Checker.Validate(
        connection.GetType() != typeof(Connection),
        new Error(CONNECTION, "Connection type invalid!")
    );
    public static void CheckConnectionType(Connection connection) => Checker.Check(ValidateConnectionType(connection));

    // Spectators -------------------------------------------------------------------------------------------------------------------------
    public const string SPECTATORS = "Game.Spectators";

    // Constants:
    public const int MAX_SPECTATORS = 100;

    public static List<Error> ValidateSpectatorCount(Game value) => Checker.Validate(
        MAX_SPECTATORS <= value.SpectatorCount,
        new Error(SPECTATORS, "Game can not have more spectators!")
    );
    public static Game CheckSpectatorCount(Game value, string? message = null) {
        Checker.Check(ValidateSpectatorCount(value), message); return value;
    }
}
