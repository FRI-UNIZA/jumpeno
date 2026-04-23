namespace Jumpeno.Client.Components;

public partial class Loader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "loader";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(AnimationHandler.ClassPreventDisabledAnimation);
    }
}
