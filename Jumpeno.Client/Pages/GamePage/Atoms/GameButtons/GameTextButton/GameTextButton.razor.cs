namespace Jumpeno.Client.Components;

public partial class GameTextButton : IDisabledComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "game-text-button";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}
