namespace Jumpeno.Client.Base;

#pragma warning disable CA1822

/// <summary>Component base to derive specific base classes.</summary>
public class Component : ComponentBase, IAsyncDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter(Name = ThemeProvider.CASCADE_APP_THEME)]
    public required BaseTheme AppTheme { get; set; }
    [Parameter]
    public bool Base { get; set; } = true;
    [Parameter]
    public string Class { get; set; } = "";
    [Parameter]
    public string Style { get; set; } = "";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Disabler? Disabler = null;
    // Lifecycle:
    private readonly LockerSlim LifeLock = new();
    // Dispose:
    public bool IsDisposing { get; private set; } = false;
    public bool IsDisposed { get; private set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public virtual CSSClass ComputeClass() => new CSSClass().Set(Class).Set(Disabler?.CSSClass);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnInitialized() {
        if (IsDisposing) return;
        Disabler = this is IDisabledComponent component ? new(component) : null;
        OnComponentInitialized();
    }
    protected override Task OnInitializedAsync() => LifeLock.TryExclusive(
        async () => {
            if (IsDisposing) return;
            await OnComponentInitializedAsync();
        }
    );

    private bool ParametersSet = false;
    protected override void OnParametersSet() {
        if (IsDisposing) return;
        OnComponentParametersSet(!ParametersSet);
        ParametersSet = true;
    }
    private bool ParametersSetAsync = false;
    protected override Task OnParametersSetAsync() => LifeLock.TryExclusive(
        async () => {
            if (IsDisposing) return;
            var firstTime = !ParametersSetAsync;
            ParametersSetAsync = true;
            await OnComponentParametersSetAsync(firstTime);
        }
    );

    protected override bool ShouldRender() => ShouldComponentRender();
    protected override void OnAfterRender(bool firstRender) {
        if (IsDisposing) return;
        OnComponentAfterRender(firstRender);
    }
    protected override Task OnAfterRenderAsync(bool firstRender) => LifeLock.TryExclusive(
        async () => {
            if (IsDisposing) return;
            if (Disabler != null) await Disabler.OnViewRender();
            await OnComponentAfterRenderAsync(firstRender);
        }
    );

    public void Dispose() {}
    public virtual async ValueTask DisposeAsync() => await LifeLock.TryExclusive(
        async () => {
            IsDisposing = true;
            OnComponentDispose();
            await OnComponentDisposeAsync();
            GC.SuppressFinalize(this);
            IsDisposed = true;
            LifeLock.DisposeUnsafe();
        }
    );

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
    protected virtual void Notify(string message, object? data = null) {}
    protected virtual async Task NotifyAsync(string message, object? data = null) => await Task.CompletedTask;
}
