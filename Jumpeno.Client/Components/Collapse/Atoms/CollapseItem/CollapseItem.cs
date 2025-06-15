namespace Jumpeno.Client.Components;

public partial class CollapseItem : IDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_COLLAPSE_ITEM = "collapse-item";
    public const string CLASS_ITEM_COLLAPSED = "item-collapsed";
    public const string CLASS_COLLAPSE_ITEM_TITLE = "collapse-item-title";
    public const string CLASS_COLLAPSE_ITEM_CONTENT_WRAP = "collapse-item-content-wrap";
    public const string CLASS_COLLAPSE_ITEM_CONTENT = "collapse-item-content";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
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
    private string ID_TITLE => $"{ID}-{CLASS_COLLAPSE_ITEM_TITLE}";
    private string ID_CONTENT => $"{ID}-{CLASS_COLLAPSE_ITEM_CONTENT}";
    protected CSSClass ComputeClass() {
        var c = new CSSClass(CLASS_COLLAPSE_ITEM);
        if (Collapsed) c.Set(CLASS_ITEM_COLLAPSED);
        return c;
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    // NOTE: Fix of auto-height transition not applied to scrollbars
    private bool RenderVar = true;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CollapseItem() => ID = IDGenerator.Generate(CLASS_COLLAPSE_ITEM);

    protected override void OnComponentAfterRender(bool firstRender) {
        if (!firstRender) RenderVar = !RenderVar;
    }

    protected override void OnComponentDispose() => Collapsed = true;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Toggle() {
        await PageLoader.Show(PAGE_LOADER_TASK.COLLAPSE, true);
        Collapsed = !Collapsed;
        StateHasChanged();
        await Task.Delay(AppTheme.TRANSITION_NORMAL);
        await PageLoader.Hide(PAGE_LOADER_TASK.COLLAPSE);
    }
}
