namespace Jumpeno.Client.Components;

public partial class BoxHeader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "box-header";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}
