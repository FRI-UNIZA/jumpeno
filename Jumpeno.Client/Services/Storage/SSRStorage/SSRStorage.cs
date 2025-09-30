namespace Jumpeno.Client.Services;

public partial class SSRStorage {
    // Storage ----------------------------------------------------------------------------------------------------------------------------
    private SSRState StorageState = null!;

    // Persistor --------------------------------------------------------------------------------------------------------------------------
    [Inject] public required PersistentComponentState Persistor { private get; set; }
    private PersistingComponentStateSubscription Subscription;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentInitialized() {
        if (!AppSettings.Prerender) { StorageState = new(); return; }
        if (AppEnvironment.IsServer) {
            StorageState = new();
            Subscription = Persistor.RegisterOnPersisting(async () => {
                Persistor.PersistAsJson(nameof(SSRStorage), StorageState);
                await Task.CompletedTask;
            });
        } else {
            Persistor.TryTakeFromJson<SSRState>(nameof(SSRStorage), out var state);
            StorageState = state ?? new();
        }
    }
    
    protected override void OnComponentDispose() {
        if (!AppSettings.Prerender) return;
        if (AppEnvironment.IsClient) return;
        Subscription.Dispose();
    }

    // State ------------------------------------------------------------------------------------------------------------------------------
    public static SSRState State => Instance().StorageState;

    // Request ----------------------------------------------------------------------------------------------------------------------------
    public static void Commit<T>(HTTPField<T> field, HTTPField<T> data) {
        if (AppEnvironment.IsClient) {
            field.Data = (T?)(object?)null;
            field.Error = false;
            field.Loading = false;
        } else {
            field.Data = data.Data;
            field.Error = data.Error;
            field.Loading = false;
        }
    }
}
