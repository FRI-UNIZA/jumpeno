namespace Jumpeno.Client.Components;

public partial class Button {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string CLASS = "button";
    public const string CLASS_NO_SHADOW = "no-shadow";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public ButtonVariant? Variant { get; set; } = ButtonVariant.Primary;
    [Parameter]
    public ButtonSize? Size { get; set; } = ButtonSize.M;
    [Parameter]
    public bool NoShadow { get; set; } = false;
    
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .SetVariant(Variant)
        .SetSize(Size)
        .Set(CLASS_NO_SHADOW, NoShadow);
    }
}
