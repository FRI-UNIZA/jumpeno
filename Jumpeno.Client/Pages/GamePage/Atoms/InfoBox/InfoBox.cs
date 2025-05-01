namespace Jumpeno.Client.Components;

public partial class InfoBox {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "info-box";
    public const string CLASS_ICON = "info-icon";
    public const string CLASS_TEXT = "info-text";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string Type { get; set; }
    [Parameter]
    public required string Theme { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ComputeClass() => ComputeClass(CLASS);
}
