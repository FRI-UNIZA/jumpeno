namespace Jumpeno.Client.Components;

public abstract class TextSpacingComponent: TextComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassSpacing = "spacing";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Spacing { get; set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassSpacing, Spacing);
}
