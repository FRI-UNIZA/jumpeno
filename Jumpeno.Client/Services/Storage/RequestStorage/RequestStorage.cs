namespace Jumpeno.Client.Services;

#pragma warning disable CS8618

public static class RequestStorage {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, object> Items = [];
    private static Func<string, object> GetItem;
    private static Action<string, object> SetItem;
    private static Func<string, bool> DeleteItem;
    
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(
        Func<string, object> getItem,
        Action<string, object> setItem,
        Func<string, bool> deleteItem
    ) {
        InitOnce.Check(nameof(RequestStorage));
        GetItem = getItem;
        SetItem = setItem;
        DeleteItem = deleteItem;
    }
    public static void Init() => Init(
        key => Items[key],
        (key, o) => Items[key] = o,
        Items.Remove
    );

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static object? GetValue(string key) {
        Checker.CheckEmptyString(key, name: "key");
        try { return GetItem(key); }
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
        SetItem(key, o);
    }

    public static bool Delete(string key) {
        Checker.CheckEmptyString(key, name: "key");
        return DeleteItem(key);
    }
}
