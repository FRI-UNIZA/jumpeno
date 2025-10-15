namespace Jumpeno.Client.Services;

public static class LocalStorage {
    public static string Get(string key) => JS.Invoke<string>(JSLocalStorage.Get, key);

    public static bool IsSet(string key) => JS.Invoke<string>(JSLocalStorage.Get, key) != null;

    public static void Set(string key, string value = "true") => JS.InvokeVoid(JSLocalStorage.Set, key, value);

    public static void Delete(string key) => JS.InvokeVoid(JSLocalStorage.Delete, key);
}
