namespace Jumpeno.Client.Layouts;

public partial class Footer {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "footer";
    public const string CLASS_DISPLAY = "display";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Display { get; set; } = true;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(SURFACE.SECONDARY, Base)
        .Set(CLASS_DISPLAY, Display);
    }
}
