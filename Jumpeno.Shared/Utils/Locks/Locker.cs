namespace Jumpeno.Shared.Utils;

public class Locker {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Semaphore Semaphore = new(1, 1);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Lock() { Semaphore.WaitOne(); }
    public void Unlock() { Semaphore.Release(); }

    // Callbacks --------------------------------------------------------------------------------------------------------------------------
    public async Task<T> Lock<T>(Func<Task<T>> callback) {
        try { Lock(); return await callback(); }
        catch { Unlock(); throw; }
    }
    public async Task Lock(Func<Task> callback) {
        try { Lock(); await callback(); }
        catch { Unlock(); throw; }
    }
    public T Lock<T>(Func<T> callback) {
        try { Lock(); return callback(); }
        catch { Unlock(); throw; }
    }
    public void Lock(Action callback) {
        try { Lock(); callback(); }
        catch { Unlock(); throw; }
    }
}
