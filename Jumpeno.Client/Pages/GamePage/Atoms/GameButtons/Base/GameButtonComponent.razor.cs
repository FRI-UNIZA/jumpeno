namespace Jumpeno.Client.Components;

public partial class GameButtonComponent : IDisabledComponent {
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
    private bool PressStarted = false;
    public bool Active { get; private set; } = false;
    private bool MouseOn = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS_ACTIVE, Active).Set(CLASS_DISABLED, Disabled);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void PressStart() { Active = true; PressStarted = true; }
    private void MouseEnter() { Active = true; MouseOn = true; }
    private void MouseLeave() { Active = false; MouseOn = false; PressStarted = false; }
    private async Task PressEnd(bool valid) {
        PressStarted = false;
        if (!Active) return;
        if (!Disabled && valid) await Action.Invoke();
        if (!MouseOn) Active = false;
    }
    private async Task PressEndMouse(MouseEventArgs e) => await PressEnd(PressStarted && e.Button == MouseButton.LEFT.Raw());
    private async Task PressEndTouch(TouchEventArgs e) => await PressEnd(true);
}
