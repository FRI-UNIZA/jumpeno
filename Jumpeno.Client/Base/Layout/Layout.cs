namespace Jumpeno.Client.Base;

public class Layout : LayoutComponentBase, IAsyncDisposable {
    // Current layout ---------------------------------------------------------------------------------------------------------------------
    public static Layout Current => RequestStorage.Get<Layout>(nameof(Layout)) ?? new Layout();
    private static void SetCurrent(Layout layout) => RequestStorage.Set(nameof(Layout), layout);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnInitialized() {
        SetCurrent(this);
        OnLayoutInitialized();
    }
    protected override sealed async Task OnInitializedAsync() => await OnLayoutInitializedAsync();
    private bool ParametersSet = false;
    protected sealed override void OnParametersSet() {
        OnLayoutParametersSet(!ParametersSet);
        ParametersSet = true;
    }
    private bool ParametersSetAsync = false;
    protected sealed override async Task OnParametersSetAsync() {
        var firstTime = !ParametersSetAsync;
        ParametersSetAsync = true;
        await OnLayoutParametersSetAsync(firstTime);
    }
    protected sealed override bool ShouldRender() => ShouldLayoutRender();
    protected sealed override void OnAfterRender(bool firstRender) => OnLayoutAfterRender(firstRender);
    protected sealed override async Task OnAfterRenderAsync(bool firstRender) => await OnLayoutfterRenderAsync(firstRender);
    public async ValueTask DisposeAsync() {
        OnLayoutDispose();
        await OnLayoutDisposeAsync();
        GC.SuppressFinalize(this);
    }

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
