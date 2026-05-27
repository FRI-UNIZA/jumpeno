namespace Jumpeno.Server.Services;

/// <summary>
/// DO NOT USER DEPRECATED
/// This class exist only for <see cref="DB"/> when we rework it it can be removed
/// </summary>
public static class RequestDisposer {
    // Request registration ---------------------------------------------------------------------------------------------------------------
    private static LinkedList<object> RequestList { get {

        var requestStorage = ServerContext.GetScopedService<RequestStorage>();
            var list = requestStorage.Get<LinkedList<object>>(RequestStorageKeys.RequestDisposerList);
        if (list is null) {
            list = [];
                requestStorage.Set(RequestStorageKeys.RequestDisposerList, list);
        }
        return list;
    } }

    // Call in middleware:
    public static async Task RequestDispose() {
        foreach (var disposable in RequestList) {
            if (disposable is IDisposable syncObject) syncObject.Dispose();
            else if (disposable is IAsyncDisposable asyncObject) await asyncObject.DisposeAsync();
        }
    }

    // Use to register request disposable objects:
    private static void RequestRegister(object instance) {
        if (!AppEnvironment.IsServer) return;
        Checker.CheckDisposable(instance); 
        RequestList.AddLast(instance);
    }
    public static void RequestRegister(IDisposable instance) => RequestRegister((object) instance);
    public static void RequestRegisterAsync(IAsyncDisposable instance) => RequestRegister(instance);
    public static void TryRequestRegister(object instance) {
        if (!Checker.IsDisposable(instance)) return;
        RequestRegister(instance);
    }
}
