namespace Jumpeno.Shared.Utils;

public static class TokenValidator {
    // Token ------------------------------------------------------------------------------------------------------------------------------
    public const string TOKEN = "Token.Token";

    public static List<Error> ValidateToken(string value) => Checker.Validate(value == null, new Error(TOKEN, Checker.FIELD_UNDEFINED));
    public static string CheckToken(string value, string? message = null) {
        Checker.CheckValues(ValidateToken(value), message); return value;
    }
}
