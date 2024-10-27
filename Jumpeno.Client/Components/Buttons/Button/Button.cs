
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
    public static string VariantClass(BUTTON_VARIANT variant) {
        switch (variant) {
            case BUTTON_VARIANT.SECONDARY:
                return "variant-secondary";
            case BUTTON_VARIANT.PRIMARY:
            default:
                return "variant-primary";
        }
    }
    public static string SizeClass(BUTTON_SIZE size) {
        switch (size) {
            case BUTTON_SIZE.S:
                return "size-s";
            case BUTTON_SIZE.M:
                return "size-m";
            case BUTTON_SIZE.L:
                return "size-l";
            default:
                return "size-m";
        }
    }
    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASSNAME);
        c.Set(VariantClass(Variant));
        c.Set(SizeClass(Size));
        return c;
    }
}
