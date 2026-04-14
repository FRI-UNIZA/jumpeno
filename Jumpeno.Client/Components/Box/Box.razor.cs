namespace Jumpeno.Client.Components;

public partial class Box {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "box";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public BoxSurface? Surface { get; set; } = BoxSurface.PrimaryBox;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base).SetSurface(Surface);
}
