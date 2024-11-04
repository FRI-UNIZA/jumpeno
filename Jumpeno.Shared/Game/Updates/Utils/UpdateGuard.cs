namespace Jumpeno.Shared.Utils;

public class UpdateGuard<T> where T : GameUpdate {
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
}
