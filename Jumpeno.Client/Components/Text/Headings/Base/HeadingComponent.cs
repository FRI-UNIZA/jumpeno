namespace Jumpeno.Client.Components;

public partial class HeadingComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "heading";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required HEADING_TYPE Type { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}
