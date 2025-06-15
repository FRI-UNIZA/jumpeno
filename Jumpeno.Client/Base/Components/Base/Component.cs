namespace Jumpeno.Client.Base;

// NOTE: Derive only specific base class
public class Component : ComponentBase, IAsyncDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme AppTheme { get; set; }
    [Parameter]
    public string Class { get; set; } = "";
    [Parameter]
    public string? Style { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected virtual CSSClass ComputeClass(string className = "") {
        var c = new CSSClass(className);
        c.Set(Class);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnInitialized() => OnComponentInitialized();
    protected override async Task OnInitializedAsync() => await OnComponentInitializedAsync();
    private bool ParametersSet = false;
    protected override void OnParametersSet() {
        OnComponentParametersSet(!ParametersSet);
        ParametersSet = true;
    }
    private bool ParametersSetAsync = false;
    protected override async Task OnParametersSetAsync() {
        var firstTime = !ParametersSetAsync;
        ParametersSetAsync = true;
        await OnComponentParametersSetAsync(firstTime);
    }
    protected override bool ShouldRender() => ShouldComponentRender();
    protected override void OnAfterRender(bool firstRender) => OnComponentAfterRender(firstRender);
    protected override async Task OnAfterRenderAsync(bool firstRender) => await OnComponentAfterRenderAsync(firstRender);
    public void Dispose() {}
    public virtual async ValueTask DisposeAsync() {
        OnComponentDispose();
        await OnComponentDisposeAsync();
        GC.SuppressFinalize(this);
    }

    // Lifecycle overrides ----------------------------------------------------------------------------------------------------------------
    protected virtual void OnComponentInitialized() {}
    protected virtual Task OnComponentInitializedAsync() => Task.CompletedTask;
    protected virtual void OnComponentParametersSet(bool firstTime) {}
    protected virtual Task OnComponentParametersSetAsync(bool firstTime) => Task.CompletedTask;
    protected virtual bool ShouldComponentRender() => true;
    protected virtual void OnComponentAfterRender(bool firstRender) {}
    protected virtual Task OnComponentAfterRenderAsync(bool firstRender) => Task.CompletedTask;
    protected virtual void OnComponentDispose() {}
    protected virtual ValueTask OnComponentDisposeAsync() => ValueTask.CompletedTask;
    
    // Notification -----------------------------------------------------------------------------------------------------------------------
    public void Notify() => StateHasChanged();
}
