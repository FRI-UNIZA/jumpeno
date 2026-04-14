namespace Jumpeno.Client.Components;

/// <summary>This component style is based on app button.</summary>
public partial class GameButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "game-button";

    /// <summary>Use this class for any icon.</summary>
    public const string CLASS_ICON = "game-button-icon";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public GameButtonVariant Variant { get; set; } = GameButtonVariant.Primary;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .SetVariant(Variant);
    }
}
