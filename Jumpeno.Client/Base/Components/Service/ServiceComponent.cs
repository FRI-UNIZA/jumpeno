namespace Jumpeno.Client.Base;

public class ServiceComponent<T> : Component {
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ServiceComponent() {
        var key = MemoryStorageKeys.ServiceComponent<T>();
        var instance = AppEnvironment.GetService<MemoryStorage>().Get<T>(key);
        if (instance is not null) throw new InvalidOperationException($"{key} already initialized!");
        AppEnvironment.GetService<MemoryStorage>().Set(key, this);
    }
    protected override sealed void OnInitialized() => base.OnInitialized();
    protected override sealed async Task OnInitializedAsync() => await base.OnInitializedAsync();
    protected override sealed void OnParametersSet() => base.OnParametersSet();
    protected override sealed async Task OnParametersSetAsync() => await base.OnParametersSetAsync();
    protected override sealed bool ShouldRender() => base.ShouldRender();
    protected override sealed void OnAfterRender(bool firstRender) => base.OnAfterRender(firstRender);
    protected override sealed async Task OnAfterRenderAsync(bool firstRender) => await base.OnAfterRenderAsync(firstRender);
    public override sealed async ValueTask DisposeAsync() => await base.DisposeAsync();

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    protected static T Instance() => AppEnvironment.MemoryStorage.Get<T>(MemoryStorageKeys.ServiceComponent<T>())!;
}
