namespace Jumpeno.Client.Constants;

public static class JSLocalStorage {
    public static readonly string ClassName = nameof(JSLocalStorage);

    public static readonly string Get = $"{ClassName}.{nameof(Get)}";
    public static readonly string Set = $"{ClassName}.{nameof(Set)}";
    public static readonly string Delete = $"{ClassName}.{nameof(Delete)}";
}
