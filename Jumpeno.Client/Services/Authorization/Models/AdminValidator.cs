namespace Jumpeno.Client.Utils;

public static class AdminValidator {
    // Email ------------------------------------------------------------------------------------------------------------------------------
    public static byte EMAIL_MAX_LENGTH => Email.MAX_LENGTH;

    public static bool IsEmail(string value) => Checker.IsEmail(value);
    public static List<Error> ValidateEmail(string value, string id = "") {
        var errors = Checker.Validate(value == null, Errors.UNDEFINED.SetID(id));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Length == 0, Errors.EMPTY.SetID(id));
        Checker.Validate(errors, !Checker.IsValidEmail(value), Errors.FORMAT.SetID(id));
        return errors;
    }
    public static string AssertEmail(string value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateEmail(value, id), exception ?? Exceptions.VALUES);
    }
}
