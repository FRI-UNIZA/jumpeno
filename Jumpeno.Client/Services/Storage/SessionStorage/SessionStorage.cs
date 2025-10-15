namespace Jumpeno.Client.Services;

public static class SessionStorage {
    public static string Get(string key) => JS.Invoke<string>(JSSessionStorage.Get, key);

    public static bool IsSet(string key) => JS.Invoke<string>(JSSessionStorage.Get, key) != null;

    public static void Set(string key, string value = "true") => JS.InvokeVoid(JSSessionStorage.Set, key, value);

    public static void Delete(string key) => JS.InvokeVoid(JSSessionStorage.Delete, key);
}
