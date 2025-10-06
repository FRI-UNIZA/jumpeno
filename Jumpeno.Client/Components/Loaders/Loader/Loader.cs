namespace Jumpeno.Client.Components;

public partial class Loader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "loader";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(AnimationHandler.CLASS_PREVENT_DISABLED_ANIMATION);
    }
}
