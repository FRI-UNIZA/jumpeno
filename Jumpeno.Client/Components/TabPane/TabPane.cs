namespace Jumpeno.Client.Components;

public partial class TabPane
{
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Key { get; set; } = string.Empty;
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentInitialized() => Key ??= IDGenerator.Generate(nameof(TabPane));
}
