namespace Jumpeno.Shared.Services;

#pragma warning disable CS8618

public static class RequestStorage {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static Dictionary<string, object> Items;
    private static Func<string, object> GetServerItem;
    private static Action<string, object> SetServerItem;
    private static Func<string, bool> DeleteServerItem;
    
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(
        Func<string, object> getServerItem, Action<string, object> setServerItem, Func<string, bool> deleteServerItem
    ) {
        if (Items is not null) throw new InvalidOperationException("RequestStorage already initialized!");
        Items = [];
        GetServerItem = getServerItem;
        SetServerItem = setServerItem;
        DeleteServerItem = deleteServerItem;
    }
    public static void Init() {
        Items = [];
        GetServerItem = (string key) => null!;
        SetServerItem = (string key, object o) => {};
        DeleteServerItem = (string key) => false;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static object? GetValue(string key) {
        Checker.CheckEmptyString(key, name: "key");
        try { return AppEnvironment.IsServer ? GetServerItem(key) : Items[key]; }
        catch { return null; }
    }
    public static T? Get<T>(string key) {
        Checker.CheckEmptyString(key, name: "key");
        return (T?) GetValue(key);
    }

    public static T Access<T>(string key, T initial) {
        var data = Get<T>(key);
        if (data != null) return data;
        data = initial!;
        Set(key, data);
        return data;
    }

    public static void Set(string key, object o) {
        Checker.CheckEmptyString(key, name: "key");
        if (AppEnvironment.IsServer) { SetServerItem(key, o); }
        else { Items[key] = o; }
    }

    public static bool Delete(string key) {
        Checker.CheckEmptyString(key, name: "key");
        return AppEnvironment.IsServer ? DeleteServerItem(key) : Items.Remove(key);
    }
}
