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
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID = IDGenerator.Generate(CLASS);
    private string ReturnFocusID = WebDocument.ID;
    private bool Loading = false;
    private readonly LockerSlim LoadLock = new();

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_NO_LOADER, NoLoader)
        .Set(CLASS_LOADING, Loading);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentDispose() => LoadLock.Dispose();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task StartLoading() => await LoadLock.Exclusive(() => {
        if (Loading) return;
        ReturnFocusID = ActionHandler.ActiveID() ?? WebDocument.ID;
        if (ReturnFocusID == "") ReturnFocusID = WebDocument.ID;
        var focusChildID = ActionHandler.FocusedChildID($"#{ID}");
        if (focusChildID == "") focusChildID = WebDocument.ID;
        if (focusChildID != null) ActionHandler.SetFocus(ID);
        Loading = true;
        StateHasChanged();
    });

    public async Task FinishLoading() => await LoadLock.Exclusive(async () => {
        if (!Loading) return;
        var hasFocus = ActionHandler.HasFocus($"#{ID}");
        Loading = false;
        StateHasChanged();
        await Task.Yield();
        if (hasFocus) ActionHandler.SetFocus(ReturnFocusID);
        ReturnFocusID = WebDocument.ID;
    });
}
