namespace Jumpeno.Client.Utils;

public static class GameValidator {
    // Code -------------------------------------------------------------------------------------------------------------------------------
    public const byte CodeLength = 4;

    public static bool IsCode(string value) => Checker.IsAlphaNum(value);
    public static bool IsCodeCase(string value) => IsCode(value) && value.ToUpper() == value;
    public static List<Error> ValidateCode(string? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value = $"{value}";
        Checker.Validate(errors, value.Length != CodeLength,
            Errors.Default.SetID(id)
            .SetInfo("Length must be I18N{length}", new() {{ "length", CodeLength }})
        );
        Checker.Validate(errors, !Checker.IsAlphaNum(value), Errors.Default.SetID(id).SetInfo("Code must be alphanumeric"));
        Checker.Validate(errors, value.ToUpper() != value, Errors.Default.SetID(id).SetInfo("Code must be uppercase"));
        return errors;
    }
    public static string AssertCode(string? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateCode(value, id), exception ?? Exceptions.Values)!;
    }

    // Name -------------------------------------------------------------------------------------------------------------------------------
    public const byte NameMinLength = 3;
    public const byte NameMaxLength = 20;

    public static bool IsName(string value) => Checker.IsAlphaNum(value, ['.', ' ']);
    public static List<Error> ValidateName(string? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id);
        value = $"{value?.Trim()}";
        Checker.Validate(
            errors,
            value.Length < NameMinLength || NameMaxLength < value.Length,
            Errors.Default.SetID(id)
            .SetInfo("Length is not between I18N{min} and I18N{max}", new() {{ "min", NameMinLength }, { "max", NameMaxLength }})
        );
        Checker.Validate(errors, !IsName(value), Errors.Default.SetID(id).SetInfo("Value contains not allowed character"));
        Checker.Validate(errors, value.Length > 0 && value[0] == '.', Errors.Default.SetID(id).SetInfo("Value must not start with a dot"));
        return errors;
    }
    public static string AssertName(string? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateName(value, id), exception ?? Exceptions.Values)!;
    }

    // Anonyms ----------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateAnonyms(bool? value, string id = "") => Checker.ValidateUndefined(value, id);
    public static bool AssertAnonyms(bool? value, string id = "", AppException? exception = null) {
        return (bool)Checker.Assert(value, ValidateAnonyms(value, id), exception ?? Exceptions.Values)!;
    }

    // Rounds -----------------------------------------------------------------------------------------------------------------------------
    public const byte MinRounds = 1;
    public const byte MaxRounds = 12;
    public static List<Error> ValidateRounds(byte? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value ??= 0;
        Checker.Validate(
            errors,
            value < MinRounds || MaxRounds < value,
            Errors.Default.SetID(id)
            .SetInfo("Number of rounds not between I18N{min} and I18N{max}", new() {{ "min", MinRounds }, { "max", MaxRounds }})
        );
        return errors;
    }
    public static byte AssertRounds(byte? value, string id = "", AppException? exception = null) {
        return (byte)Checker.Assert(value, ValidateRounds(value, id), exception ?? Exceptions.Values)!;
    }

    // Capacity ---------------------------------------------------------------------------------------------------------------------------
    public const byte MinCapacity = 2;
    public const byte MaxCapacity = 10;

    public static List<Error> ValidateCapacity(byte? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value ??= 0;
        Checker.Validate(
            errors,
            value < MinCapacity || MaxCapacity < value,
            Errors.Default.SetID(id)
            .SetInfo("Capacity not between I18N{min} and I18N{max}", new() {{ "min", MinCapacity }, { "max", MaxCapacity }})
        );
        return errors;
    }
    public static byte AssertCapacity(byte? value, string id = "", AppException? exception = null) {
        return (byte)Checker.Assert(value, ValidateCapacity(value, id), exception ?? Exceptions.Values)!;
    }

    // Display mode -----------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateDisplayMode(DisplayMode? value, string id = "") => Checker.ValidateUndefined(value, id);
    public static DisplayMode AssertDisplayMode(DisplayMode? value, string id = "", AppException? exception = null) {
        return (DisplayMode)Checker.Assert(value, ValidateDisplayMode(value, id), exception ?? Exceptions.Values)!;
    }

    // Game mode --------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateGameMode(GameMode? value, string id = "") => Checker.ValidateUndefined(value, id);
    public static GameMode AssertGameMode(GameMode? value, string id = "", AppException? exception = null) {
        return (GameMode)Checker.Assert(value, ValidateGameMode(value, id), exception ?? Exceptions.Values)!;
    }

    // Spectators -------------------------------------------------------------------------------------------------------------------------
    public static int MaxSpectators => AppSettings.Game.MaxSpectators;

    // Instances --------------------------------------------------------------------------------------------------------------------------
    public static int MaxInstances => AppSettings.Game.MaxInstances;

    public static List<Error> ValidateMaxInstances(int? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id);
        Checker.Validate(
            errors,
            MaxInstances <= value,
            Errors.Default.SetID(id).SetInfo("Maximum active games limit exceeded!")
        );
        return errors;
    }
    public static int AssertMaxInstances(int? value, string id = "", AppException? exception = null) {
        return (int)Checker.Assert(value, ValidateMaxInstances(value, id), exception ?? Exceptions.Values)!;
    }
}
