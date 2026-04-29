namespace Jumpeno.Client.Components;

public partial class GameWaiting {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "game-waiting";
    public const string ClassFinished = "finished";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Finished { get; set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base).Set(ClassFinished, Finished);
}
