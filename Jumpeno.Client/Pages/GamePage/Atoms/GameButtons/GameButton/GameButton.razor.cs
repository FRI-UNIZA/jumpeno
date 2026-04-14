namespace Jumpeno.Client.Components;

/// <summary>This component style is based on app button.</summary>
public partial class GameButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "game-button";

    /// <summary>Use this class for any icon.</summary>
    public const string CLASS_ICON = "game-button-icon";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public GAME_BUTTON_VARIANT Variant { get; set; } = GAME_BUTTON_VARIANT.PRIMARY;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .SetVariant(Variant);
    }
}
