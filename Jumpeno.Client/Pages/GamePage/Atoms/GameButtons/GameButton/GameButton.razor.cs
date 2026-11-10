namespace Jumpeno.Client.Components;

/// <summary>This component style is based on app button.</summary>
public partial class GameButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "game-button";

    /// <summary>Use this class for any icon.</summary>
    public const string ClassIcon = "game-button-icon";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public GameButtonVariant Variant { get; set; } = GameButtonVariant.Primary;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .SetVariant(Variant);
    }
}
