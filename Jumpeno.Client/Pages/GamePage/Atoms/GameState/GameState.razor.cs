namespace Jumpeno.Client.Components;

public partial class GameState {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "game-state";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}
