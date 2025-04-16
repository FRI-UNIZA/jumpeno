namespace Jumpeno.Shared.Utils;

#pragma warning disable CA1816

public class LockerSlim : IDisposable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly SemaphoreSlim Semaphore = new(1, 1);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public LockerSlim() => Disposer = new(this, Semaphore.Dispose);
    private readonly Disposer Disposer;
    public void Dispose() => Disposer.Dispose();
    ~LockerSlim() => Disposer.Final();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Lock() => await Semaphore.WaitAsync();
    public void Unlock() => Semaphore.Release();
    // [Dispose] Exception prone:
    public async Task TryLock() { try { await Lock(); } catch {} }
    public void TryUnlock() { try { Semaphore.Release(); } catch {} }

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
    // [Dispose] Exception prone:
    public async Task TryExclusive(Action callback) {
        try { await Exclusive(callback); } catch {}
    }
    public async Task<T> TryExclusive<T>(Func<T> callback, T fallback = default!) {
        try { return await Exclusive(callback); } catch { return fallback; }
    }
    public async Task TryExclusive(Func<Task> callback) {
        try { await Exclusive(callback); } catch {}
    }
    public async Task<T> TryExclusive<T>(Func<Task<T>> callback, T fallback = default!) {
        try { return await Exclusive(callback); } catch { return fallback; }
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
    // [Dispose] Exception prone:
    public async Task TryExclusive(Action<LockToken> callback) {
        try { await Exclusive(callback); } catch {}
    }
    public async Task<T> TryExclusive<T>(Func<LockToken, T> callback, T fallback = default!) {
        try { return await Exclusive(callback); } catch { return fallback; }
    }
    public async Task TryExclusive(Func<LockToken, Task> callback) {
        try { await Exclusive(callback); } catch {}
    }
    public async Task<T> TryExclusive<T>(Func<LockToken, Task<T>> callback, T fallback = default!) {
        try { return await Exclusive(callback); } catch { return fallback; }
    }
}
