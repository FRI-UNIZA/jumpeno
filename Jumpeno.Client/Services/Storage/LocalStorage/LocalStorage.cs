namespace Jumpeno.Client.Services;

public static class LocalStorage {
    public static string Get(string key)
    {
        HTTP.EnforceSync();
        return JS.Invoke<string>(JSLocalStorage.Get, key);
    }

    public static bool IsSet(string key)
    {
        HTTP.EnforceSync();
        return JS.Invoke<string>(JSLocalStorage.Get, key) != null;
    }
    public static void Set(string key, string value = "true")
    {
        HTTP.EnforceSync();
        JS.InvokeVoid(JSLocalStorage.Set, key, value);
    }

    public static void Delete(string key)
    {
        HTTP.EnforceSync();
        JS.InvokeVoid(JSLocalStorage.Delete, key);
    }
}
