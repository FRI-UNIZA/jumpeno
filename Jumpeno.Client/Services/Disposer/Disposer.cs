namespace Jumpeno.Client.Services;

#pragma warning disable CA1816

public class Disposer {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly object Instance;
    private readonly Action? FreeUnmanagedResources;
    private readonly Func<Task>? FreeUnmanagedResourcesAsync;
    private readonly Action? FreeManagedResources;
    private readonly Func<Task>? FreeManagedResourcesAsync;
    private bool Disposed = false;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private Disposer(
        object instance,
        Action? unmanaged = null, Func<Task>? unmanagedAsync = null,
        Action? managed = null,  Func<Task>? managedAsync = null
    ) {
        Instance = instance;
        FreeUnmanagedResources = unmanaged;
        FreeUnmanagedResourcesAsync = unmanagedAsync;
        FreeManagedResources = managed;
        FreeManagedResourcesAsync = managedAsync;
    }
    public Disposer(object instance, Action unmanaged) : this(instance, unmanaged, null, null, null) {}
    public Disposer(object instance, Action unmanaged, Action managed) : this(instance, unmanaged, null, managed, null) {}
    public Disposer(object instance, Action unmanaged, Func<Task> managed) : this(instance, unmanaged, null, null, managed) {}
    public Disposer(object instance, Func<Task> unmanaged) : this(instance, null, unmanaged, null, null) {}
    public Disposer(object instance, Func<Task> unmanaged, Action managed) : this(instance, null, unmanaged, managed, null) {}
    public Disposer(object instance, Func<Task> unmanaged, Func<Task> managed) : this(instance, null, unmanaged, null, managed) {}

    // Execution --------------------------------------------------------------------------------------------------------------------------
    private void Exec(bool disposing) {
        // 1) Dispose unmanaged:
        if (Disposed) return; Disposed = true;
        FreeUnmanagedResources?.Invoke();
        // 2) Dispose managed:
        if (!disposing) return;
        FreeManagedResources?.Invoke();
    }

    private async Task ExecAsync(bool disposing) {
        // 1) Dispose unmanaged:
        if (Disposed) return; Disposed = true;
        FreeUnmanagedResources?.Invoke();
        if (FreeUnmanagedResourcesAsync != null) await FreeUnmanagedResourcesAsync();
        // 2) Dispose managed:
        if (!disposing) return;
        FreeManagedResources?.Invoke();
        if (FreeManagedResourcesAsync != null) await FreeManagedResourcesAsync();
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        Exec(true);
        GC.SuppressFinalize(Instance);
    }

    public async Task DisposeAsync() {
        await ExecAsync(true);
        GC.SuppressFinalize(Instance);
    }

    public void Final() => Exec(false);
    public async void FinalAsync() => await ExecAsync(false);

}
