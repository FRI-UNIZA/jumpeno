namespace Jumpeno.Client.Utils;

public static class TokenValidator {
    // Token ------------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateToken(string value, string id = "") => Checker.Validate(value == null, Errors.Default.SetID(id).SetInfo(Fields.Undefined));
    public static string AssertToken(string value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateToken(value, id), exception ?? Exceptions.Values);
    }
}
