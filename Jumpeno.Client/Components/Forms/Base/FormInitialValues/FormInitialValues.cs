namespace Jumpeno.Client.Utils;

// Key ------------------------------------------------------------------------------------------------------------------------------------
public interface IFormInitialValuesKey {
    static abstract string Key { get; }
}

// InitialValues --------------------------------------------------------------------------------------------------------------------------
public abstract class FormInitialValues<K, T>
    where K : IFormInitialValuesKey
    where T : FormInitialValues<K, T>, new()
{
    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static T Read() {
        T values;
        try { values = JsonSerializer.Deserialize<T>(SessionStorageUtils.Get(K.Key)) ?? new T(); }
        catch { values = new T(); }
        SessionStorageUtils.Set(K.Key, JsonSerializer.Serialize(values));
        return values;
    }

    public static bool AreSet() => SessionStorageUtils.IsSet(K.Key);

    public static void Set(T values) => SessionStorageUtils.Set(K.Key, JsonSerializer.Serialize(values));

    public static void Delete() => SessionStorageUtils.Delete(K.Key);

    // Commit -----------------------------------------------------------------------------------------------------------------------------
    public void Commit(Action<T> save) {
        save((T)this);
        Set((T)this);
    }
}
