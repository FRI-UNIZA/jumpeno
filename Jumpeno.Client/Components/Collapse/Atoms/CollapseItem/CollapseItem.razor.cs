namespace Jumpeno.Client.Components;

public partial class CollapseItem {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassCollapseItem = "collapse-item";
    public const string ClassItemCollapsed = "item-collapsed";
    public const string ClassCollapseItemTitle = "collapse-item-title";
    public const string ClassCollapseItemContentWrap = "collapse-item-content-wrap";
    public const string ClassCollapseItemContent = "collapse-item-content";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required RenderFragment Title { get; set; }
    [Parameter]
    public required RenderFragment Content { get; set; }
    [Parameter]
    public string? Label { get; set; } = null;
    [Parameter]
    public bool Collapsed { get; set; } = true;
    
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private readonly string ID;
    private string IDTitle => $"{ID}-{ClassCollapseItemTitle}";
    private string IDContent => $"{ID}-{ClassCollapseItemContent}";

    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassCollapseItem, Base)
        .Set(ClassItemCollapsed, Collapsed);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CollapseItem() => ID = IDGenerator.Generate(ClassCollapseItem);

    // NOTE: Fix of auto-height transition not applied to scrollbars
    private bool RenderVar = true;
    protected override void OnComponentAfterRender(bool firstRender) {
        if (!firstRender) RenderVar = !RenderVar;
    }

    protected override void OnComponentDispose() => Collapsed = true;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Toggle() {
        await PageLoader.Show(PageLoaderTask.Collapse, true);
        Collapsed = !Collapsed;
        StateHasChanged();
        await Task.Delay(AppTheme.TransitionNormal);
        await PageLoader.Hide(PageLoaderTask.Collapse, false);
    }
}
