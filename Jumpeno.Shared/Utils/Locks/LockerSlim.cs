namespace Jumpeno.Shared.Utils;

public class LockerSlim {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly SemaphoreSlim Semaphore = new(1, 1);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Lock() => await Semaphore.WaitAsync();
    public void Unlock() => Semaphore.Release();

    // Callbacks --------------------------------------------------------------------------------------------------------------------------
    public async Task Exclusive(Action callback) {
        try { await Lock(); callback(); }
        finally { Unlock(); }
    }
    public async Task<T> Exclusive<T>(Func<T> callback) {
        try { await Lock(); return callback(); }
        finally { Unlock(); }
    }
    public async Task Exclusive(Func<Task> callback) {
        try { await Lock(); await callback(); }
        finally { Unlock(); }
    }
    public async Task<T> Exclusive<T>(Func<Task<T>> callback) {
        try { await Lock(); return await callback(); }
        finally { Unlock(); }
    }

    // Token callbacks --------------------------------------------------------------------------------------------------------------------
    public async Task Exclusive(Action<LockToken> callback) {
        var token = new LockToken(Unlock);
        try { await Lock(); callback(token); }
        finally { token.Unlock(); }
    }
    public async Task<T> Exclusive<T>(Func<LockToken, T> callback) {
        var token = new LockToken(Unlock);
        try { await Lock(); return callback(token); }
        finally { token.Unlock(); }
    }
    public async Task Exclusive(Func<LockToken, Task> callback) {
        var token = new LockToken(Unlock);
        try { await Lock(); await callback(token); }
        finally { token.Unlock(); }
    }
    public async Task<T> Exclusive<T>(Func<LockToken, Task<T>> callback) {
        var token = new LockToken(Unlock);
        try { await Lock(); return await callback(token); }
        finally { token.Unlock(); }
    }
}
