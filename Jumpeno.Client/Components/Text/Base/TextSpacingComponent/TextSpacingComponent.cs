namespace Jumpeno.Client.Components;

public abstract class TextSpacingComponent: TextComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_SPACING = "spacing";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Spacing { get; set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS_SPACING, Spacing);
}
