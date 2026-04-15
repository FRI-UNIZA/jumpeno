namespace Jumpeno.Client.Constants;

public static class RequestStorageKeys {
    // Database:
    public const string DB = $"{nameof(DB)}";
    public const string RequestDisposerList = $"{nameof(RequestDisposer)}_list";
    public static string TokenAccess => TokenType.Access.String();
}
