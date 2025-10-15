namespace Jumpeno.Client.Components;

public partial class LoadArea {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "load-area";
    public const string CLASS_CONTENT = "load-area-content";
    public const string CLASS_LOADER_WRAP = "load-area-loader-wrap";
    public const string CLASS_LOADER = "load-area-loader";
    public const string CLASS_NO_LOADER = "no-loader";
    public const string CLASS_LOADING = "loading";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool NoLoader { get; set; } = false;
    [Parameter]
    public bool InitLoading { get; set; } = false;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID = IDGenerator.Generate(CLASS);
    private string ReturnFocusID = WebDocument.ID;
    // State:
    public bool Loading { get; private set; } = false;
    private readonly LockerSlim LoadLock = new();
    // Dispose:
    private bool Disposed = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_NO_LOADER, NoLoader)
        .Set(CLASS_LOADING, Loading);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        Loading = InitLoading;
    }

    protected override void OnComponentDispose() {
        LoadLock.Dispose();
        Disposed = true;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task StartLoading() {
        if (Disposed) return;
        await LoadLock.Exclusive(() => {
            if (Loading) return;
            ReturnFocusID = ActionHandler.ActiveID() ?? WebDocument.ID;
            if (ReturnFocusID == "") ReturnFocusID = WebDocument.ID;
            var focusChildID = ActionHandler.FocusedChildID($"#{ID}");
            if (focusChildID == "") focusChildID = WebDocument.ID;
            if (focusChildID != null) ActionHandler.SetFocus(ID);
            Loading = true;
            StateHasChanged();
        });
    }

    public async Task FinishLoading() {
        if (Disposed) return;
        await LoadLock.Exclusive(async () => {
            if (!Loading) return;
            var hasFocus = ActionHandler.HasFocus($"#{ID}");
            Loading = false;
            StateHasChanged();
            await Task.Yield();
            if (hasFocus) ActionHandler.SetFocus(ReturnFocusID);
            ReturnFocusID = WebDocument.ID;
        });
    }
}
