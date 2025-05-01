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

    public string ComputeClass() {
        var c = new CSSClass(CLASS);
        if (Active) c.Set(CLASS_ACTIVE);
        c.Set(Class);
        return c;
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
