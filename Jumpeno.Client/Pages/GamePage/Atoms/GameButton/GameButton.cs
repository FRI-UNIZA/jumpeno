namespace Jumpeno.Client.Components;

public partial class GameButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "game-button";
    public const string CLASS_ACTIVE = "active";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required EmptyDelegate Action { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Active { get; private set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_ACTIVE, Active);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void PressStart() => Active = true;
    private void MouseLeave() => Active = false;
    private async Task PressEnd() {
        if (!Active) return;
        await Action.Invoke();
        Active = false;
    }
}
