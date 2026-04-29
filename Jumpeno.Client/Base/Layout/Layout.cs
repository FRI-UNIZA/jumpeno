namespace Jumpeno.Client.Base;

#pragma warning disable CA1822

public class Layout : LayoutComponentBase, IAsyncDisposable {
    // Current layout ---------------------------------------------------------------------------------------------------------------------
    public static Layout Current => RequestStorage.Get<Layout>(RequestStorages.Layout) ?? new Layout();
    private static void SetCurrent(Layout layout) => RequestStorage.Set(RequestStorages.Layout, layout);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Lifecycle:
    private readonly LockerSlim LifeLock = new();
    // Dispose:
    public bool IsDisposing { get; private set; } = false;
    public bool IsDisposed { get; private set; } = false;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnInitialized() {
        if (IsDisposing) return;
        SetCurrent(this);
        OnLayoutInitialized();
    }
    protected override sealed Task OnInitializedAsync() => LifeLock.TryExclusive(
        async () => {
            if (IsDisposing) return;
            await OnLayoutInitializedAsync();
        }
    );

    private bool ParametersSet = false;
    protected sealed override void OnParametersSet() {
        if (IsDisposing) return;
        OnLayoutParametersSet(!ParametersSet);
        ParametersSet = true;
    }
    private bool ParametersSetAsync = false;
    protected sealed override Task OnParametersSetAsync() => LifeLock.TryExclusive(
        async () => {
            if (IsDisposing) return;
            var firstTime = !ParametersSetAsync;
            ParametersSetAsync = true;
            await OnLayoutParametersSetAsync(firstTime);
        }
    );

    protected sealed override bool ShouldRender() => ShouldLayoutRender();
    protected sealed override void OnAfterRender(bool firstRender) {
        if (IsDisposing) return;
        OnLayoutAfterRender(firstRender);
    }
    protected sealed override Task OnAfterRenderAsync(bool firstRender) => LifeLock.TryExclusive(
        async () => {
            if (IsDisposing) return;
            await OnLayoutfterRenderAsync(firstRender);
        }
    );

    public void Dispose() {}
    public async ValueTask DisposeAsync() => await LifeLock.TryExclusive(
        async () => {
            IsDisposing = true;
            OnLayoutDispose();
            await OnLayoutDisposeAsync();
            GC.SuppressFinalize(this);
            IsDisposed = true;
            LifeLock.DisposeUnsafe();
        }
    );

    // Lifecycle overrides ----------------------------------------------------------------------------------------------------------------
    protected virtual void OnLayoutInitialized() {}
    protected virtual Task OnLayoutInitializedAsync() => Task.CompletedTask;
    protected virtual void OnLayoutParametersSet(bool firstTime) {}
    protected virtual Task OnLayoutParametersSetAsync(bool firstTime) => Task.CompletedTask;
    protected virtual bool ShouldLayoutRender() => true;
    protected virtual void OnLayoutAfterRender(bool firstRender) {}
    protected virtual Task OnLayoutfterRenderAsync(bool firstRender) => Task.CompletedTask;
    protected virtual void OnLayoutDispose() {}
    protected virtual ValueTask OnLayoutDisposeAsync() => ValueTask.CompletedTask;

    // Notification -----------------------------------------------------------------------------------------------------------------------
    public void Notify() => StateHasChanged();

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    protected RenderFragment? Render() => AuthPage.Render(Body);
}
