namespace Jumpeno.Client.Atoms;

public partial class CollapseItem : IDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_COLLAPSE_ITEM = "collapse-item";
    public const string CLASS_ITEM_COLLAPSED = "item-collapsed";
    public const string CLASS_COLLAPSE_ITEM_TITLE = "collapse-item-title";
    public const string CLASS_COLLAPSE_ITEM_CONTENT_WRAP = "collapse-item-content-wrap";
    public const string CLASS_COLLAPSE_ITEM_CONTENT = "collapse-item-content";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme Theme { get; set; }
    [Parameter]
    public required RenderFragment Title { get; set; }
    [Parameter]
    public required RenderFragment Content { get; set; }
    [Parameter]
    public string? Label { get; set; } = null;
    [Parameter]
    public bool Collapsed { get; set; } = true;
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID;
    private string ID_TITLE() { return $"{ID}-{CLASS_COLLAPSE_ITEM_TITLE}"; }
    private string ID_CONTENT() { return $"{ID}-{CLASS_COLLAPSE_ITEM_CONTENT}"; }
    protected CSSClass ComputeClass() {
        var c = new CSSClass(CLASS_COLLAPSE_ITEM);
        if (Collapsed) c.Set(CLASS_ITEM_COLLAPSED);
        return c;
    }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public CollapseItem() {
        ID = ComponentService.GenerateID(CLASS_COLLAPSE_ITEM);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        Collapsed = true;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private async Task Toggle() {
        await PageLoader.Show(PAGE_LOADER_TASK.COLLAPSE, true);
        Collapsed = !Collapsed;
        StateHasChanged();
        await Task.Delay(Theme.TRANSITION_NORMAL);
        await PageLoader.Hide(PAGE_LOADER_TASK.COLLAPSE);
    }
}
