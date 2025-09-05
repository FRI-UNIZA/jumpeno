namespace Jumpeno.Client.Components;

public partial class Collapse {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "collapse";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public COLLAPSE_SURFACE? Surface { get; set; } = COLLAPSE_SURFACE.PRIMARY_COLLAPSE;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base).SetSurface(Surface);
}
