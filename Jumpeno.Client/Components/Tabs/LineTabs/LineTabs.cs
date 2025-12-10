namespace Jumpeno.Client.Components;

public partial class LineTabs
{
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter] 
    public string? DefaultActiveKey { get; set; }
    [Parameter] 
    public string? ActiveKey { get; set; }
    // Style:
    [Parameter]
    public TABS_POSITION Position { get; set; } = TABS_POSITION.TOP;
    [Parameter]
    public bool Centered { get; set; }
    [Parameter]
    public string TabBarClass { get; set; } = string.Empty;
    // Events:
    [Parameter]
    public EventCallback<string?> OnChanged { get; set; }
    // Content:
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
}
