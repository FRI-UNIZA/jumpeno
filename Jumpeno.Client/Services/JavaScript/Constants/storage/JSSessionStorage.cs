namespace Jumpeno.Client.Constants;

public static class JSSessionStorage {
    public static readonly string ClassName = nameof(JSSessionStorage);

    public static readonly string Get = $"{ClassName}.{nameof(Get)}";
    public static readonly string Set = $"{ClassName}.{nameof(Set)}";
    public static readonly string Delete = $"{ClassName}.{nameof(Delete)}";
}
