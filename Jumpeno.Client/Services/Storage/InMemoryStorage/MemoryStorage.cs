namespace Jumpeno.Client.Services;

public class MemoryStorage
{
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Dictionary<string, object> Items = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------

    public T? Get<T>(string key)
    {
        Checker.CheckEmptyString(key, name: nameof(key));
        if (Items.TryGetValue(key, out object? value) && value is T typedValue)
            return typedValue;
        return default;
    }

    public T Access<T>(string key, T initial)
    {
        var data = Get<T>(key);
        if (data != null) return data;
        data = initial!;
        Set(key, data);
        return data;
    }

    public void Set(string key, object o)
    {
        Checker.CheckEmptyString(key, name: "key");
        Items[key] = o;
    }

    public bool Delete(string key)
    {
        Checker.CheckEmptyString(key, name: "key");
        return Items.Remove(key);
    }
}
