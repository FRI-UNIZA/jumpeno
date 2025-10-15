namespace Jumpeno.Client.Utils;

#pragma warning disable CA1816

public class Locker : IDisposable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Semaphore Semaphore = new(1, 1);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Locker() => Disposer = new(this, Semaphore.Dispose);
    private readonly Disposer Disposer;
    public void Dispose() => Disposer.Dispose();
    ~Locker() => Disposer.Final();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Lock() => Semaphore.WaitOne();
    public void Unlock() => Semaphore.Release();
    // [Dispose] Exception prone:
    public void TryLock() { try { Lock(); } catch {} }
    public void TryUnlock() { try { Semaphore.Release(); } catch {} }

    // Callbacks --------------------------------------------------------------------------------------------------------------------------
    public void Exclusive(Action callback) {
        try { Lock(); callback(); }
        finally { TryUnlock(); }
    }
    public T Exclusive<T>(Func<T> callback) {
        try { Lock(); return callback(); }
        finally { TryUnlock(); }
    }
    public async Task Exclusive(Func<Task> callback) {
        try { Lock(); await callback(); }
        finally { TryUnlock(); }
    }
    public async Task<T> Exclusive<T>(Func<Task<T>> callback) {
        try { Lock(); return await callback(); }
        finally { TryUnlock(); }
    }
    // [Dispose] Exception prone:
    public void TryExclusive(Action callback) {
        try { Exclusive(callback); } catch {}
    }
    public T TryExclusive<T>(Func<T> callback, T fallback = default!) {
        try { return Exclusive(callback); } catch { return fallback; }
    }
    public async Task TryExclusive(Func<Task> callback) {
        try { await Exclusive(callback); } catch {}
    }
    public async Task<T> TryExclusive<T>(Func<Task<T>> callback, T fallback = default!) {
        try { return await Exclusive(callback); } catch { return fallback; }
    }

    // Token callbacks --------------------------------------------------------------------------------------------------------------------
    public void Exclusive(Action<LockToken> callback) {
        var token = new LockToken(TryUnlock);
        try { Lock(); callback(token); }
        finally { token.Unlock(); }
    }
    public T Exclusive<T>(Func<LockToken, T> callback) {
        var token = new LockToken(TryUnlock);
        try { Lock(); return callback(token); }
        finally { token.Unlock(); }
    }
    public async Task Exclusive(Func<LockToken, Task> callback) {
        var token = new LockToken(TryUnlock);
        try { Lock(); await callback(token); }
        finally { token.Unlock(); }
    }
    public async Task<T> Exclusive<T>(Func<LockToken, Task<T>> callback) {
        var token = new LockToken(TryUnlock);
        try { Lock(); return await callback(token); }
        finally { token.Unlock(); }
    }
    // [Dispose] Exception prone:
    public void TryExclusive(Action<LockToken> callback) {
        try { Exclusive(callback); } catch {}
    }
    public T TryExclusive<T>(Func<LockToken, T> callback, T fallback = default!) {
        try { return Exclusive(callback); } catch { return fallback; }
    }
    public async Task TryExclusive(Func<LockToken, Task> callback) {
        try { await Exclusive(callback); } catch {}
    }
    public async Task<T> TryExclusive<T>(Func<LockToken, Task<T>> callback, T fallback = default!) {
        try { return await Exclusive(callback); } catch { return fallback; }
    }
}
