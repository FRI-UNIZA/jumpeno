namespace Jumpeno.Client.Base;

public class ServiceComponent<T>: ComponentBase {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string Key;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public ServiceComponent() {
        Key = GetType().Name;
        var instance = RequestStorage.Get<T>(Key);
        if (instance is not null) throw new InvalidOperationException($"{Key} already initialized!");
        RequestStorage.Set(Key, this);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    protected static T Instance() {
        return RequestStorage.Get<T>(typeof(T).Name)!;
    }
}