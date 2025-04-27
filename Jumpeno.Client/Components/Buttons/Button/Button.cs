namespace Jumpeno.Client.Components;

public partial class Button {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "button";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public BUTTON_VARIANT Variant { get; set; } = BUTTON_VARIANT.PRIMARY;
    [Parameter]
    public BUTTON_SIZE Size { get; set; } = BUTTON_SIZE.M;
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASSNAME);
        c.Set(Variant.String());
        c.Set(Size.String());
        return c;
    }
}
