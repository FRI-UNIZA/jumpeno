namespace Jumpeno.Client.Components;

public partial class Collapse {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "collapse";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public CollapseSurface? Surface { get; set; } = CollapseSurface.PRIMARY_COLLAPSE;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base).SetSurface(Surface);
}
