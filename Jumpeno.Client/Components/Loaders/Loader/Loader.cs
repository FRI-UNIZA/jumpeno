namespace Jumpeno.Client.Components;

public partial class Loader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "loader";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass() {
        CSSClass c = ComputeClass(CLASSNAME);
        c.Set(AnimationHandler.IMUNE_ANIMATION_CLASSNAME);
        return c;
    }
}
