namespace Jumpeno.Client.Utils;

public class UpdateGuard<T> where T : NetworkUpdate {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private T? LastUpdate = null;

    // Checks -----------------------------------------------------------------------------------------------------------------------------
    public bool ShouldUpdate(ref T? lastUpdate, T nextUpdate) {
        var shouldUpdate = lastUpdate == null || lastUpdate.ID < nextUpdate.ID;
        if (shouldUpdate) lastUpdate = nextUpdate;
        return shouldUpdate;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(T update, Action callback) {
        if (!ShouldUpdate(ref LastUpdate, update)) return false;
        callback();
        return true;
    }

    public bool Update(T update, Func<bool> callback) {
        if (!ShouldUpdate(ref LastUpdate, update)) return false;
        return callback();
    }

    public void Reset() {
        LastUpdate = null;
    }
}
