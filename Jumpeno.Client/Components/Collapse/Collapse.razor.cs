namespace Jumpeno.Client.Components;

public partial class Collapse {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "collapse";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public CollapseSurface? Surface { get; set; } = CollapseSurface.PrimaryCollapse;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base).SetSurface(Surface);
}
