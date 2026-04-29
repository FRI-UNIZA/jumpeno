namespace Jumpeno.Client.Components;

public partial class Box {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "box";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public BoxSurface? Surface { get; set; } = BoxSurface.PrimaryBox;
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base).SetSurface(Surface);
}
