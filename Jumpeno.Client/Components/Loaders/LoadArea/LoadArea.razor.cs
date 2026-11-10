namespace Jumpeno.Client.Components;

public partial class LoadArea {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "load-area";
    public const string ClassContent = "load-area-content";
    public const string ClassLoaderWrap = "load-area-loader-wrap";
    public const string ClassLoader = "load-area-loader";
    public const string ClassNoStyle = "no-style";
    public const string ClassNoLoader = "no-loader";
    public const string ClassLoading = "loading";
    // Min loading time:
    public const uint MinLoading = 150; // ms

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoadAreaViewModel ViewModel { get; set; }
    [Parameter]
    public required LoadAreaType Type { get; set; } = LoadAreaType.Focusable;
    [Parameter]
    public required string Label { get; set; }
    [Parameter]
    public string LoadLabel { get; set; } = I18N.T("Loading");
    [Parameter]
    public string? RoleDescription { get; set; } = null;
    [Parameter]
    public bool NoStyle { get; set; } = false;
    [Parameter]
    public bool NoLoader { get; set; } = false;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string RestoreFocusID = WebDocument.ID;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(ClassNoStyle, NoStyle)
        .Set(ClassNoLoader, NoLoader)
        .Set(ClassLoading, ViewModel.Loading);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (!ViewModel<LoadArea>.Connect(ViewModel, this)) return;
        RestoreFocusID = ViewModel.ID;
    }

    protected override async ValueTask OnComponentDisposeAsync() => await ViewModel.OnViewDispose();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void StartLoading(LoadAreaViewModel.MessageStartData data) {
        try {
            if (IsDisposed) return;
            if (Type == LoadAreaType.NoFocus) { StateHasChanged(); return; }
            RestoreFocusID = ActionHandler.ActiveID() ?? ViewModel.ID;
            if (RestoreFocusID == "") RestoreFocusID = ViewModel.ID;
            var focusChildId = ActionHandler.FocusedChildID($"#{ViewModel.ID}");
            if (focusChildId == "") focusChildId = ViewModel.ID;
            if (focusChildId == ViewModel.ID) focusChildId = null;
            if (focusChildId != null) ActionHandler.SetFocus(ViewModel.ID, preventScroll: data.PreventScroll);
            StateHasChanged();
        } catch {}
    }

    private async Task FinishLoading(LoadAreaViewModel.MessageFinishData data) {
        try {
            if (IsDisposed) return;
            if (Type == LoadAreaType.NoFocus) { StateHasChanged(); return; }
            try {
                if (!data.RestoreFocus) { StateHasChanged(); return; }
                var hasFocus = ViewModel.HasFocus();
                if (!hasFocus) { StateHasChanged(); return; }
                if (RestoreFocusID == ViewModel.ID) { StateHasChanged(); return; }
                StateHasChanged();
                await Task.Yield();
                ActionHandler.SetFocus(RestoreFocusID, ViewModel.ID, data.PreventScroll);
            } finally {
                RestoreFocusID = ViewModel.ID;
            }
        } catch {}
    }

    private async Task RestoreFocus(LoadAreaViewModel.MessageRestoreData data) {
        RestoreFocusID = data.ID;
        await FinishLoading(new(true, data.PreventScroll));
    }

    private void SetRestoreID(LoadAreaViewModel.MessageSetRestoreIDData data) => RestoreFocusID = data.ID;

    // Notification -----------------------------------------------------------------------------------------------------------------------
    protected override void Notify(string message, object? data = null) {
        if (!AppEnvironment.IsClient) return;
        switch (message) {
            case LoadAreaViewModel.MessageSetRestoreId: SetRestoreID((LoadAreaViewModel.MessageSetRestoreIDData)data!); break;
        }
    }

    protected override async Task NotifyAsync(string message, object? data = null) {
        if (!AppEnvironment.IsClient) return;
        switch (message) {
            case LoadAreaViewModel.MessageStart: StartLoading((LoadAreaViewModel.MessageStartData)data!); break;
            case LoadAreaViewModel.MessageFinish: await FinishLoading((LoadAreaViewModel.MessageFinishData)data!); break;
            case LoadAreaViewModel.MessageRestore: await RestoreFocus((LoadAreaViewModel.MessageRestoreData)data!); break;
        }
    }
}
