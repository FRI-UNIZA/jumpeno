namespace Jumpeno.Client.Components;

public partial class GameState {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "game-state";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}
