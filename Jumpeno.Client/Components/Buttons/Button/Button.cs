namespace Jumpeno.Client.Components;

public partial class Button {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "button";
    public const string CLASS_NO_SHADOW = "no-shadow";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public BUTTON_VARIANT Variant { get; set; } = BUTTON_VARIANT.PRIMARY;
    [Parameter]
    public bool NoShadow { get; set; } = false;
    [Parameter]
    public BUTTON_SIZE Size { get; set; } = BUTTON_SIZE.M;
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASS);
        c.Set(Variant.String());
        if (NoShadow) c.Set(CLASS_NO_SHADOW);
        c.Set(Size.String());
        return c;
    }
}
