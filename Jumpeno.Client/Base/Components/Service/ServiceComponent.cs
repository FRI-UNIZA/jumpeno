namespace Jumpeno.Client.Base;

public class ServiceComponent<T> : Component {
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ServiceComponent() {
        var key = REQUEST_STORAGE.SERVICE_COMPONENT<T>();
        var instance = RequestStorage.Get<T>(key);
        if (instance is not null) throw new InvalidOperationException($"{key} already initialized!");
        RequestStorage.Set(key, this);
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
    protected static T Instance() => RequestStorage.Get<T>(REQUEST_STORAGE.SERVICE_COMPONENT<T>())!;
}
