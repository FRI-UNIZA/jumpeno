namespace Jumpeno.Client.Services;

public class StaticService<T> {
    protected static T Instance() {
        var key = REQUEST_STORAGE.STATIC_SERVICE<T>();
        var instance = RequestStorage.Get<T>(key);
        if (instance is null) {
            instance = Reflex.CreateInstance<T>(typeof(T));
            RequestStorage.Set(key, instance);
            Disposer.TryRequestRegister(instance);
        }
        return instance;
    }
}
