namespace Jumpeno.Client.Constants;

public static class Chars {
    public const string AlphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string AlphaLower = "abcdefghijklmnopqrstuvwxyz";
    public static readonly string Alpha = $"{AlphaUpper}{AlphaLower}";

    public const string Num = "0123456789";
    public const string Special = "!@#$%^&*()_-+={[}]:;'<,>.?/";

    public static readonly string AlphaUpperNum = $"{AlphaUpper}{Num}";
    public static readonly string AlphaLowerNum = $"{AlphaLower}{Num}";
    public static readonly string AlphaNum = $"{AlphaUpper}{AlphaLower}{Num}";
}
