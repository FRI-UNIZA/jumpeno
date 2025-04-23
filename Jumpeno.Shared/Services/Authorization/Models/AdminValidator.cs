namespace Jumpeno.Shared.Utils;

public static class AdminValidator {
    // Email ------------------------------------------------------------------------------------------------------------------------------
    public const string EMAIL = "Admin.Email";

    public static byte EMAIL_MAX_LENGTH => Constants.EMAIL.MAX_LENGTH;

    public static List<Error> ValidateEmail(string value) {
        var errors = Checker.Validate(value == null, new Error(EMAIL, Checker.FIELD_UNDEFINED));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Length == 0, new Error(EMAIL, Checker.FIELD_EMPTY));
        Checker.Validate(errors, !Checker.IsValidEmail(value), new Error(EMAIL, Checker.FIELD_FORMAT));
        return errors;
    }
    public static string CheckEmail(string value, string? message = null) {
        Checker.CheckValues(ValidateEmail(value), message); return value;
    }
}
