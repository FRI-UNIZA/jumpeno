namespace Jumpeno.Client.Utils;

public static class GameValidator {
    // Code -------------------------------------------------------------------------------------------------------------------------------
    public const byte CODE_LENGTH = 4;

    public static List<Error> ValidateCode(string? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value = $"{value}";
        Checker.Validate(errors, value.Length != CODE_LENGTH,
            ERROR.DEFAULT.SetID(id)
            .SetInfo("Length must be equal to I18N{length}", new() {{ "length", CODE_LENGTH }})
        );
        Checker.Validate(errors, !Checker.IsAlphaNum(value), ERROR.DEFAULT.SetID(id).SetInfo("Code must be alphanumeric"));
        Checker.Validate(errors, value.ToUpper() != value, ERROR.DEFAULT.SetID(id).SetInfo("Code must be uppercase"));
        return errors;
    }
    public static string AssertCode(string? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateCode(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    // Name -------------------------------------------------------------------------------------------------------------------------------
    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 20;

    public static List<Error> ValidateName(string value, string id = "") {
        value = value.Trim();
        var errors = Checker.Validate(
            value.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < value.Length,
            ERROR.DEFAULT.SetID(id)
            .SetInfo("Length is not between I18N{min} and I18N{max}", new() {{ "min", NAME_MIN_LENGTH }, { "max", NAME_MAX_LENGTH }})
        );
        Checker.Validate(errors, !Checker.IsAlphaNum(value, ['.', ' ']), ERROR.DEFAULT.SetID(id).SetInfo("Value contains not allowed character"));
        Checker.Validate(errors, value.Length > 0 && value[0] == '.', ERROR.DEFAULT.SetID(id).SetInfo("Value must not start with a dot"));
        return errors;
    }
    public static string AssertName(string value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateName(value, id), exception ?? EXCEPTION.VALUES);
    }

    // Capacity ---------------------------------------------------------------------------------------------------------------------------
    public const byte MIN_CAPACITY = 2;
    public const byte MAX_CAPACITY = 10;

    public static List<Error> ValidateCapacity(byte value, string id = "") => Checker.Validate(
        value < MIN_CAPACITY || MAX_CAPACITY < value,
        ERROR.DEFAULT.SetID(id)
        .SetInfo("Capacity not between I18N{min} and I18N{max}", new() {{ "min", MIN_CAPACITY }, { "max", MAX_CAPACITY }})
    );
    public static byte AssertCapacity(byte value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateCapacity(value, id), exception ?? EXCEPTION.VALUES);
    }

    public static List<Error> ValidatePlayerCount(Game value, string id = "") => Checker.Validate(
        value.Capacity <= value.ActivePlayersCount,
        ERROR.DEFAULT.SetID(id).SetInfo("The game is currently full!")
    );
    public static Game AssertPlayerCount(Game value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidatePlayerCount(value, id), exception ?? EXCEPTION.VALUES);
    }

    // Connection -------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateConnectionType(Connection value, string id = "") => Checker.Validate(
        value.GetType() != typeof(Connection),
        ERROR.DEFAULT.SetID(id).SetInfo("Connection type invalid!")
    );
    public static Connection AssertConnectionType(Connection value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateConnectionType(value, id), exception ?? EXCEPTION.VALUES);
    }

    // Spectators -------------------------------------------------------------------------------------------------------------------------
    public const int MAX_SPECTATORS = 100;

    public static List<Error> ValidateSpectatorCount(Game value, string id = "") => Checker.Validate(
        MAX_SPECTATORS <= value.SpectatorCount,
        ERROR.DEFAULT.SetID(id).SetInfo("Game can not have more spectators!")
    );
    public static Game AssertSpectatorCount(Game value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateSpectatorCount(value, id), exception ?? EXCEPTION.VALUES);
    }
}
