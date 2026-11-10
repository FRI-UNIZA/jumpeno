namespace Jumpeno.Client.Components;

public partial class GameButtonComponent : IDisabledComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassContent = "game-button-content";
    // States:
    public const string ClassActive = "active";
    public const string ClassDisabled = "disabled";

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
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassActive, Active).Set(ClassDisabled, Disabled);

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
    private async Task PressEndMouse(MouseEventArgs e) => await PressEnd(PressStarted && e.Button == MouseButton.Left.Raw());
    private async Task PressEndTouch(TouchEventArgs e) => await PressEnd(true);
}
