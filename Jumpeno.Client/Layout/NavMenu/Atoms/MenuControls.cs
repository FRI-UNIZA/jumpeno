namespace Jumpeno.Client.Atoms;

public partial class MenuControls {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "menu-controls";
    public const string CLASS_MOBILE = "mobile";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Mobile { get; set; } = false;

    // Attributes -------------------------------------------------------------------------------------------------------------------------    
    private CSSClass ComputeClass() {
        var c = new CSSClass(CLASS);
        if (Mobile) c.Set(CLASS_MOBILE);
        return c;
    }
}
