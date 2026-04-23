namespace Jumpeno.Client.Components;

public partial class BoxHeader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "box-header";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}
