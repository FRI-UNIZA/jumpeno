namespace Jumpeno.Client.Components;

public partial class GameTextButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "game-text-button";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}
