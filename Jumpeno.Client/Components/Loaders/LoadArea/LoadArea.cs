namespace Jumpeno.Client.Components;

public partial class LoadArea {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "load-area";
    public const string CLASS_CONTENT = "load-area-content";
    public const string CLASS_LOADER_WRAP = "load-area-loader-wrap";
    public const string CLASS_LOADER = "load-area-loader";
    public const string CLASS_NO_STYLE = "no-style";
    public const string CLASS_NO_LOADER = "no-loader";
    public const string CLASS_LOADING = "loading";
    // Min loading time:
    public const uint MIN_LOADING = 150; // ms

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoadAreaViewModel ViewModel { get; set; }
    [Parameter]
    public required LOAD_AREA_TYPE Type { get; set; } = LOAD_AREA_TYPE.FOCUSABLE;
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
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_NO_STYLE, NoStyle)
        .Set(CLASS_NO_LOADER, NoLoader)
        .Set(CLASS_LOADING, ViewModel.Loading);
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
            if (Type == LOAD_AREA_TYPE.NO_FOCUS) { StateHasChanged(); return; }
            RestoreFocusID = ActionHandler.ActiveID() ?? ViewModel.ID;
            if (RestoreFocusID == "") RestoreFocusID = ViewModel.ID;
            var focusChildID = ActionHandler.FocusedChildID($"#{ViewModel.ID}");
            if (focusChildID == "") focusChildID = ViewModel.ID;
            if (focusChildID == ViewModel.ID) focusChildID = null;
            if (focusChildID != null) ActionHandler.SetFocus(ViewModel.ID, preventScroll: data.PreventScroll);
            StateHasChanged();
        } catch {}
    }

    private async Task FinishLoading(LoadAreaViewModel.MessageFinishData data) {
        try {
            if (IsDisposed) return;
            if (Type == LOAD_AREA_TYPE.NO_FOCUS) { StateHasChanged(); return; }
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
            case LoadAreaViewModel.MESSAGE_SET_RESTORE_ID: SetRestoreID((LoadAreaViewModel.MessageSetRestoreIDData)data!); break;
        }
    }

    protected override async Task NotifyAsync(string message, object? data = null) {
        if (!AppEnvironment.IsClient) return;
        switch (message) {
            case LoadAreaViewModel.MESSAGE_START: StartLoading((LoadAreaViewModel.MessageStartData)data!); break;
            case LoadAreaViewModel.MESSAGE_FINISH: await FinishLoading((LoadAreaViewModel.MessageFinishData)data!); break;
            case LoadAreaViewModel.MESSAGE_RESTORE: await RestoreFocus((LoadAreaViewModel.MessageRestoreData)data!); break;
        }
    }
}
