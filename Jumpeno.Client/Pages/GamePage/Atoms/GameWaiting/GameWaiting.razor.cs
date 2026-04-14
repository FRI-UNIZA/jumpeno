namespace Jumpeno.Client.Components;

public partial class GameWaiting {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "game-waiting";
    public const string CLASS_FINISHED = "finished";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Finished { get; set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base).Set(CLASS_FINISHED, Finished);
}
