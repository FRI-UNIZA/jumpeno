namespace Jumpeno.Client.Components;

public partial class GameButtonComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_CONTENT = "game-button-content";
    // States:
    public const string CLASS_ACTIVE = "active";
    public const string CLASS_DISABLED = "disabled";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required EmptyDelegate Action { get; set; }
    [Parameter]
    public bool Disabled { get; set; } = false;
    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Active { get; private set; } = false;
    private bool MouseOn = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS_ACTIVE, Active).Set(CLASS_DISABLED, Disabled);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void PressStart() => Active = true;
    private void MouseEnter() { Active = true; MouseOn = true; }
    private void MouseLeave() { Active = false; MouseOn = false; }
    private async Task PressEnd() {
        if (!Active) return;
        if (!Disabled) await Action.Invoke();
        if (!MouseOn) Active = false;
    }
}
