namespace Jumpeno.Client.Components;

public partial class Button {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string ClassName = "button";
    public const string ClassNoShadow = "no-shadow";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public ButtonVariant? Variant { get; set; } = ButtonVariant.Primary;
    [Parameter]
    public ButtonSize? Size { get; set; } = ButtonSize.M;
    [Parameter]
    public bool NoShadow { get; set; } = false;
    
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .SetVariant(Variant)
        .SetSize(Size)
        .Set(ClassNoShadow, NoShadow);
    }
}
