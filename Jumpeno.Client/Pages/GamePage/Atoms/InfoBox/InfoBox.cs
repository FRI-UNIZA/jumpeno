namespace Jumpeno.Client.Components;

public partial class InfoBox {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "info-box";
    public const string CLASS_ICON = "info-icon";
    public const string CLASS_TEXT = "info-text";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Class { get; set; } = "";
    [Parameter]
    public required string Type { get; set; }
    [Parameter]
    public required string Theme { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ComputeClass() {
        var c = new CSSClass(CLASS);
        c.Set(Class);
        return c;
    }
}
