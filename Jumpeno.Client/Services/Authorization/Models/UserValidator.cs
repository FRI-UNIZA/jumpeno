namespace Jumpeno.Client.Utils;

public static class UserValidator {
    // ID ---------------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateID(Guid? value, string id = "") {
        return Checker.Validate(value == null, ERROR.UNDEFINED.SetID(id));
    }
    public static Guid AssertID(Guid? value, string id = "", AppException? exception = null) {
        return (Guid) Checker.Assert(value, ValidateID(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    public static List<Error> ValidateID(string value, string id = "") {
        return Checker.Validate(value == null, ERROR.UNDEFINED.SetID(id));
    }
    public static string AssertID(string value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateID(value, id), exception ?? EXCEPTION.VALUES);
    }

    // Email ------------------------------------------------------------------------------------------------------------------------------
    public static byte EMAIL_MAX_LENGTH => EMAIL.MAX_LENGTH;

    public static bool IsEmail(string value) => Checker.IsEmail(value);
    public static List<Error> ValidateEmail(string? value, string id = "") {
        var errors = Checker.Validate(value == null, ERROR.UNDEFINED.SetID(id));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Length == 0, ERROR.EMPTY.SetID(id));
        Checker.Validate(errors, !Checker.IsValidEmail(value), ERROR.FORMAT.SetID(id));
        return errors;
    }
    public static string AssertEmail(string? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateEmail(value, id), exception ?? EXCEPTION.VALUES)!;
    }

    // Name -------------------------------------------------------------------------------------------------------------------------------
    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 13;

    public static bool IsName(string value) => Checker.IsAlphaNum(value);
    public static List<Error> ValidateName(string? value, bool checkUnknown = true, string id = "") {
        var errors = Checker.Validate(value == null, ERROR.UNDEFINED.SetID(id));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < value.Length,
            ERROR.DEFAULT.SetID(id)
            .SetInfo("Length is not between I18N{min} and I18N{max}", new() {{ "min", NAME_MIN_LENGTH }, { "max", NAME_MAX_LENGTH }})
        );
        if (errors.Count > 0) return errors;
        if (checkUnknown) Checker.Validate(errors, value.ToLower() == User.NAME_UNKNOWN.ToLower(), ERROR.DEFAULT.SetID(id).SetInfo("Name forbidden"));
        Checker.Validate(errors, !Checker.IsAlphaNum(value), ERROR.DEFAULT.SetID(id).SetInfo("Name must be alphanumeric"));
        Checker.Validate(errors, value.Length > 0 && char.IsDigit(value[0]), ERROR.DEFAULT.SetID(id).SetInfo("Name must not start with number"));
        return errors;
    }
    public static string AssertName(string? value, bool checkUnknown = true, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateName(value, checkUnknown, id), exception ?? EXCEPTION.VALUES)!;
    }

    public static List<Error> ValidateUnknown(User value, string id = "") => Checker.Validate(
        User.UNKNOWN.Equals(value), ERROR.DEFAULT.SetID(id).SetInfo("User undefined!")
    );
    public static User AssertUnknown(User value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateUnknown(value, id), exception ?? EXCEPTION.VALUES);
    }

    // Password ---------------------------------------------------------------------------------------------------------------------------
    public const byte PASSWORD_MIN_LENGTH = 6;
    public const byte PASSWORD_MAX_LENGTH = 30;
    public const byte PASSWORD_GENERATOR_MIN_LENGTH = 8;
    public const byte PASSWORD_GENERATOR_MAX_LENGTH = 12;

    public static bool IsPassword(string value) => Checker.IsPassword(value);
    public static List<Error> ValidatePassword(string value, string id = "") {
        var errors = Checker.Validate(value == null, ERROR.UNDEFINED.SetID(id));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Trim() == "", ERROR.EMPTY.SetID(id));
        Checker.Validate(errors, value.Length < PASSWORD_MIN_LENGTH || PASSWORD_MAX_LENGTH < value.Length,
            ERROR.DEFAULT.SetID(id)
            .SetInfo("Length is not between I18N{min} and I18N{max}", new() {{ "min", PASSWORD_MIN_LENGTH }, { "max", PASSWORD_MAX_LENGTH }})
        );
        Checker.Validate(errors, !Checker.IsPassword(value), ERROR.DEFAULT.SetID(id).SetInfo("Invalid characters"));
        return errors;
    }
    public static string AssertPassword(string value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidatePassword(value, id), exception ?? EXCEPTION.VALUES);
    }

    public static List<Error> ValidateConfirmPassword(string value, string password, string id = "") {
        List<Error> errors = [];
        Checker.Validate(errors, value.Trim() == "", ERROR.EMPTY.SetID(id));
        Checker.Validate(errors, value != password, ERROR.NOT_MATCH().SetID(id));
        return errors;
    }
    public static string AssertConfirmPassword(string value, string password, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateConfirmPassword(value, password, id), exception ?? EXCEPTION.VALUES);
    }

    // Skin -------------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateSkin(SKIN? value, string id = "") => Checker.Validate(
        value == null, ERROR.UNDEFINED.SetID(id)
    );
    public static SKIN AssertSkin(SKIN? value, string id = "", AppException? exception = null) {
        return (SKIN) Checker.Assert(value, ValidateSkin(value, id), exception ?? EXCEPTION.VALUES)!;
    }
    
    // Registered user --------------------------------------------------------------------------------------------------------------------
    public static List<Error> Validate(User? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id);
        errors.AddRange(ValidateID(value?.ID, id));
        errors.AddRange(ValidateEmail(value?.Email, id));
        errors.AddRange(ValidateName(value?.Name, true, id));
        errors.AddRange(ValidateSkin(value?.Skin, id));
        return errors;
    }
    public static User Assert(User? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, Validate(value, id), exception ?? EXCEPTION.VALUES)!;
    }
}
