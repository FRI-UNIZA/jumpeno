namespace Jumpeno.Client.Services;

public class StaticService<T> {
    protected static T Instance() {
        var name = typeof(T).Name;
        var instance = RequestStorage.Get<T>(name);
        if (instance is null) {
            instance = Reflex.CreateInstance<T>(typeof(T));
            RequestStorage.Set(name, instance);
            Disposer.TryRequestRegister(instance);
        }
        return instance;
    }
}
