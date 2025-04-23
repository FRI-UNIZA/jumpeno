namespace Jumpeno.Shared.Constants;

public static class CHARS {
    public const string ALPHA_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string ALPHA_LOWER = "abcdefghijklmnopqrstuvwxyz";
    public static readonly string ALPHA = $"{ALPHA_UPPER}{ALPHA_LOWER}";

    public const string NUM = "0123456789";

    public static readonly string ALPHA_UPPER_NUM = $"{ALPHA_UPPER}{NUM}";
    public static readonly string ALPHA_LOWER_NUM = $"{ALPHA_LOWER}{NUM}";
    public static readonly string ALPHA_NUM = $"{ALPHA_UPPER}{ALPHA_LOWER}{NUM}";
}
