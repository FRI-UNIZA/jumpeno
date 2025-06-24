namespace Jumpeno.Client.Utils;

public static class Form {
    public static string Of<T>() => IDGenerator.Generate(typeof(T).Name);
}
