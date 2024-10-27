namespace Jumpeno.Client.Services;

public static class LocalStorage {
    public static string Get(string key) {
        return JS.Invoke<string>(JSLocalStorage.Get, key);
    }

    public static void Set(string key, string value) {
        JS.InvokeVoid(JSLocalStorage.Set, key, value);
    }

    public static void Delete(string key) {
        JS.InvokeVoid(JSLocalStorage.Delete, key);
    }
}
