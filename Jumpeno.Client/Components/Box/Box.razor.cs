namespace Jumpeno.Client.Components;

public partial class Box {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "box";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public BOX_SURFACE? Surface { get; set; } = BOX_SURFACE.PRIMARY_BOX;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base).SetSurface(Surface);
}
