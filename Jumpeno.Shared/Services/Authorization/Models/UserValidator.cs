namespace Jumpeno.Shared.Utils;

public static class UserValidator {
    // ID ---------------------------------------------------------------------------------------------------------------------------------
    public const string ID = "User.ID";

    public static List<Error> ValidateID(Guid? value) {
        var errors = Checker.Validate(value == null, new Error(ID, Checker.FIELD_UNDEFINED));
        return errors;
    }
    public static Guid CheckID(Guid? value, string? message = null) {
        Checker.CheckValues(ValidateID(value), message); return (Guid) value!;
    }

    public static List<Error> ValidateID(string value) {
        var errors = Checker.Validate(value == null, new Error(ID, Checker.FIELD_UNDEFINED));
        return errors;
    }
    public static string CheckID(string value, string? message = null) {
        Checker.CheckValues(ValidateID(value), message); return value;
    }

    // Email ------------------------------------------------------------------------------------------------------------------------------
    public const string EMAIL = "User.Email";

    public static byte EMAIL_MAX_LENGTH => Constants.EMAIL.MAX_LENGTH;

    public static List<Error> ValidateEmail(string? value) {
        var errors = Checker.Validate(value == null, new Error(EMAIL, Checker.FIELD_UNDEFINED));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Length == 0, new Error(EMAIL, Checker.FIELD_EMPTY));
        Checker.Validate(errors, !Checker.IsValidEmail(value), new Error(EMAIL, Checker.FIELD_FORMAT));
        return errors;
    }
    public static string CheckEmail(string? value, string? message = null) {
        Checker.CheckValues(ValidateEmail(value), message); return value!;
    }

    // Name -------------------------------------------------------------------------------------------------------------------------------
    public const string NAME = "User.Name";

    // Constants:
    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 13;

    public static List<Error> ValidateName(string? value, bool checkUnknown = true) {
        var errors = Checker.Validate(value == null, new Error(NAME, Checker.FIELD_UNDEFINED));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < value.Length, new Error(
            NAME,
            "Length is not between I18N{min} and I18N{max}",
            new() {{"min", $"{NAME_MIN_LENGTH}"}, {"max", $"{NAME_MAX_LENGTH}"}}
        ));
        if (errors.Count > 0) return errors;
        if (checkUnknown) Checker.Validate(errors, value.ToLower() == User.NAME_UNKNOWN.ToLower(), new Error(NAME, "Name forbidden"));
        Checker.Validate(errors, !Checker.IsAlphaNum(value), new Error(NAME, "Name must be alphanumeric"));
        Checker.Validate(errors, value.Length > 0 && char.IsDigit(value[0]), new Error(NAME, "Name must not start with number"));
        return errors;
    }
    public static string CheckName(string? value, string? message = null, bool checkUnknown = true) {
        Checker.CheckValues(ValidateName(value, checkUnknown), message); return value!;
    }

    // Password ---------------------------------------------------------------------------------------------------------------------------
    public const string PASSWORD = "User.Password";
    public const string PASSWORD_CONFIRM = "User.PasswordConfirm";

    // Constants:
    public const byte PASSWORD_MIN_LENGTH = 6;
    public const byte PASSWORD_MAX_LENGTH = 30;
    public const byte PASSWORD_GENERATOR_MIN_LENGTH = 8;
    public const byte PASSWORD_GENERATOR_MAX_LENGTH = 12;

    public static List<Error> ValidatePassword(string value) {
        var errors = Checker.Validate(value == null, new Error(PASSWORD, Checker.FIELD_UNDEFINED));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Trim() == "", new Error(PASSWORD, Checker.FIELD_EMPTY));
        Checker.Validate(errors, value.Length < PASSWORD_MIN_LENGTH || PASSWORD_MAX_LENGTH < value.Length, new Error(
            PASSWORD,
            "Length is not between I18N{min} and I18N{max}",
            new() {{"min", $"{PASSWORD_MIN_LENGTH}"}, {"max", $"{PASSWORD_MAX_LENGTH}"}}
        ));
        Checker.Validate(errors, !Checker.IsPassword(value), new Error(PASSWORD, "Invalid characters"));
        return errors;
    }
    public static string CheckPassword(string value, string? message = null) {
        Checker.CheckValues(ValidatePassword(value), message); return value;
    }

    // Skin -------------------------------------------------------------------------------------------------------------------------------
    public const string SKIN = "User.Skin";

    public static List<Error> ValidateSkin(SKIN? value) => Checker.Validate(value == null, new Error(SKIN, Checker.FIELD_UNDEFINED));
    public static SKIN CheckSkin(SKIN? value, string? message = null) {
        Checker.CheckValues(ValidateSkin(value), message); return (SKIN) value!;
    }

    // Unknown ----------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateUnknown(User value) => Checker.Validate(User.UNKNOWN.Equals(value), new Error("User undefined!"));
    public static User CheckUnknown(User value, string? message = null) {
        Checker.Check(ValidateUnknown(value), message); return value;
    }
    
    // Registered -------------------------------------------------------------------------------------------------------------------------
    public const string USER = "User";
    public static List<Error> Validate(User? value) {
        var errors = Checker.ValidateUndefined(USER, value);
        errors.AddRange(ValidateID(value?.ID));
        errors.AddRange(ValidateEmail(value?.Email));
        errors.AddRange(ValidateName(value?.Name));
        errors.AddRange(ValidateSkin(value?.Skin));
        return errors;
    }
    public static User Check(User? value, string? message = null) {
        Checker.Check(Validate(value), message); return value!;
    }
}
