namespace Jumpeno.Shared.Utils;

public class Locker {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Semaphore Semaphore = new(1, 1);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Lock() => Semaphore.WaitOne();
    public void Unlock() => Semaphore.Release();

    // Callbacks --------------------------------------------------------------------------------------------------------------------------
    public void Exclusive(Action callback) {
        try { Lock(); callback(); }
        finally { Unlock(); }
    }
    public T Exclusive<T>(Func<T> callback) {
        try { Lock(); return callback(); }
        finally { Unlock(); }
    }
    public async Task Exclusive(Func<Task> callback) {
        try { Lock(); await callback(); }
        finally { Unlock(); }
    }
    public async Task<T> Exclusive<T>(Func<Task<T>> callback) {
        try { Lock(); return await callback(); }
        finally { Unlock(); }
    }

    // Token callbacks --------------------------------------------------------------------------------------------------------------------
    public void Exclusive(Action<LockToken> callback) {
        var token = new LockToken(Unlock);
        try { Lock(); callback(token); }
        finally { token.Unlock(); }
    }
    public T Exclusive<T>(Func<LockToken, T> callback) {
        var token = new LockToken(Unlock);
        try { Lock(); return callback(token); }
        finally { token.Unlock(); }
    }
    public async Task Exclusive(Func<LockToken, Task> callback) {
        var token = new LockToken(Unlock);
        try { Lock(); await callback(token); }
        finally { token.Unlock(); }
    }
    public async Task<T> Exclusive<T>(Func<LockToken, Task<T>> callback) {
        var token = new LockToken(Unlock);
        try { Lock(); return await callback(token); }
        finally { token.Unlock(); }
    }
}
