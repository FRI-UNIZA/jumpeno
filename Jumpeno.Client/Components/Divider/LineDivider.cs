namespace Jumpeno.Client.Components;

public partial class LineDivider
{
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Text { get; set; } = "";
    [Parameter]
    public DIVIDER_ORIENTATION Orientation { get; set; } = DIVIDER_ORIENTATION.CENTER;
    [Parameter]
    public bool Plain { get; set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set("line-divider", Base);
}
