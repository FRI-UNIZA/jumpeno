namespace Jumpeno.Client.Utils;

public static class AdminValidator {
    // Email ------------------------------------------------------------------------------------------------------------------------------
    public static byte EmailMaxLength => Email.MaxLength;

    public static bool IsEmail(string value) => Checker.IsEmail(value);
    public static List<Error> ValidateEmail(string value, string id = "") {
        var errors = Checker.Validate(value == null, Errors.Undefined.SetID(id));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Length == 0, Errors.Empty.SetID(id));
        Checker.Validate(errors, !Checker.IsValidEmail(value), Errors.Format.SetID(id));
        return errors;
    }
    public static string AssertEmail(string value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateEmail(value, id), exception ?? Exceptions.Values);
    }
}
