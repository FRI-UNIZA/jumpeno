namespace Jumpeno.Shared.Utils;

public static class TokenValidator {
    // Token ------------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateToken(string value, string id = "") => Checker.Validate(value == null, ERROR.DEFAULT.SetID(id).SetInfo(FIELD.UNDEFINED));
    public static string AssertToken(string value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateToken(value, id), exception ?? EXCEPTION.VALUES);
    }
}
