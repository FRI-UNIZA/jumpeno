namespace Jumpeno.Client.Base;

public class ServiceComponent<T> : Component {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string Key;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ServiceComponent() {
        Key = GetType().Name;
        var instance = RequestStorage.Get<T>(Key);
        if (instance is not null) throw new InvalidOperationException($"{Key} already initialized!");
        RequestStorage.Set(Key, this);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    protected static T Instance() => RequestStorage.Get<T>(typeof(T).Name)!;
}
