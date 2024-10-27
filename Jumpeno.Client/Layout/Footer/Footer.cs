namespace Jumpeno.Client.Layout;

public partial class Footer {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_DISPLAY = "display";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Display { get; set; } = true;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        var c = new CSSClass();
        if (Display) c.Set(CLASS_DISPLAY);
        return c;
    }
}
