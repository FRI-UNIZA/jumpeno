namespace Jumpeno.Client.Components;

public partial class GameScreen {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly DotNetObjectReference<GameScreen> Ref;
    private GameCanvas Canvas = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set("game-screen", Base).Set(VM.CSSClass());

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameScreen() => Ref = DotNetObjectReference.Create(this);

    protected override async Task OnComponentAfterRenderAsync(bool firstRender) {
        if (!firstRender) return;
        if (VM.IsWatching) {
            await Render();
            await VM.AddAfterUpdatesListener(AfterUpdates);
        }
        if (VM.IsPlayer) {
            await VM.StartPing();
            await Window.AddKeyDownEventListener(Ref, JS_OnKeyDown);
            await Window.AddKeyUpEventListener(Ref, JS_OnKeyUp);
            await Window.AddMouseUpEventListener(Ref, JS_OnMouseUp);
        }
        await Animator.AddAnimator(Ref, JS_OnAnimationFrame);
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        if (!AppEnvironment.IsServer) {
            if (VM.IsWatching) {
                await VM.RemoveAfterUpdatesListener(AfterUpdates);
            }
            if (VM.IsPlayer) {
                await VM.StopPing();
                await Window.RemoveKeyDownEventListener(Ref, JS_OnKeyDown);
                await Window.RemoveKeyUpEventListener(Ref, JS_OnKeyUp);
                await Window.RemoveMouseUpEventListener(Ref, JS_OnMouseUp);
            }
            await Animator.RemoveAnimator(Ref, JS_OnAnimationFrame);
        }
        await ControlLock.DisposeSafe();
        await MouseReleaseKeyEventLock.DisposeSafe();
        Ref.Dispose();
        GC.SuppressFinalize(this);
    }
    
    // Updates ----------------------------------------------------------------------------------------------------------------------------
    // 1) Apply server updates:
    private async Task Update() => await VM.ExecuteUpdates();

    // 2) After all server updates:
    public async Task AfterUpdates() {
        var deltaT = await VM.Game.Clock.AwaitDelta();
        VM.Game.Update(VM.Game.NewTimeFlowUpdate(deltaT));
    }

    // Controls ---------------------------------------------------------------------------------------------------------------------------
    // Arrows:
    private readonly List<GAME_CONTROLS> ArrowsPressed = [];
    private GAME_CONTROLS? LastArrowPressed = null;
    // Space:
    private (bool Pressed, DateTime? At) Space = (false, null);
    private DateTime? LastSpacePressedAt = null;
    // Lock:
    private readonly LockerSlim ControlLock = new();

    // Display controls:
    private CSSClass GameControlsClass() {
        var c = new CSSClass("game-controls");
        if (!VM.ControlsDisplayed) c.Set("hidden");
        return c;
    }
    private CSSClass ControlClass(GAME_CONTROLS control) {
        var c = new CSSClass("control");
        switch (control) {
            case GAME_CONTROLS.SPACE: c.Set("space"); break;
            case GAME_CONTROLS.LEFT: c.Set("left"); break;
            case GAME_CONTROLS.RIGHT: c.Set("right"); break;
        }
        if (IsPressed(control)) c.Set("pressed");
        return c;
    }

    // Save pressed keys:
    private async Task PressKey(GAME_CONTROLS control) {
        await ControlLock.TryExclusive(() => {
            switch (control) {
                case GAME_CONTROLS.SPACE:
                    if (Space.Pressed) break;
                    Space = (true, DateTime.UtcNow);
                break;
                case GAME_CONTROLS.LEFT:
                case GAME_CONTROLS.RIGHT:
                    if (ArrowsPressed.Contains(control)) break;
                    ArrowsPressed.Add(control);
                break;
            }
        });
    }
    private Func<Task> TouchKeyEvent(GAME_CONTROLS control) => () => PressKey(control);
    private Func<MouseEventArgs, Task> MouseTouchKeyEvent(GAME_CONTROLS control)
    => e => MouseReleaseKeyEventLock.TryExclusive(async () => {
        if (e.Button != MOUSE_BUTTON.LEFT.Raw()) return;
        await PressKey(control);
        MouseReleaseKeyEvent = () => MouseReleaseKeyEventLock.TryExclusive(
            async () => {
                await ReleaseKey(control);
                MouseReleaseKeyEvent = () => Task.CompletedTask;
            }
        );
    });
    [JSInvokable]
    public async Task JS_OnKeyDown(WindowKeyEvent e) {
        if (e.Repeat) return;
        if (GameControlsExtension.Get(e.Key) is not GAME_CONTROLS control) return;
        await PressKey(control);
    }

    // Save released keys:
    private async Task ReleaseKey(GAME_CONTROLS control) {
        await ControlLock.TryExclusive(() => {
            switch (control) {
                case GAME_CONTROLS.SPACE:
                    Space = (false, Space.At);
                break;
                case GAME_CONTROLS.LEFT:
                case GAME_CONTROLS.RIGHT:
                    ArrowsPressed.Remove(control);
                break;
            }
        });
    }
    private Func<Task> ReleaseKeyEvent(GAME_CONTROLS control) => () => ReleaseKey(control);
    private Func<Task> MouseReleaseKeyEvent = () => Task.CompletedTask;
    private readonly LockerSlim MouseReleaseKeyEventLock = new();
    [JSInvokable]
    public async Task JS_OnKeyUp(WindowKeyEvent e) {
        if (e.Repeat) return;
        if (GameControlsExtension.Get(e.Key) is not GAME_CONTROLS control) return;
        await ReleaseKey(control);
    }
    [JSInvokable]
    public async Task JS_OnMouseUp(WindowMouseEvent e) {
        if (e.Button == MOUSE_BUTTON.LEFT) await MouseReleaseKeyEvent();
    }

    // Check pressed key:
    protected bool IsPressed(GAME_CONTROLS control) {
        switch (control) {
            case GAME_CONTROLS.SPACE:
                return Space.Pressed;
            case GAME_CONTROLS.LEFT:
            case GAME_CONTROLS.RIGHT:
                return ArrowsPressed.Contains(control);
            default:
                return false;
        }
    }
    protected async Task<bool> IsPressedAsync(GAME_CONTROLS control) {
        return await ControlLock.TryExclusive(() => IsPressed(control), false);
    }

    // Send pressed keys to the server:
    private async Task Control() {
        if (VM.Player == null || !VM.Player.IsAlive) return;
        await ControlLock.TryExclusive(async () => {
            // 1) Arrows:
            KeyUpdate update = VM.Game.NewKeyUpdate(VM.Player.ID, []);
            if (ArrowsPressed.Count > 0) {
                if (LastArrowPressed != ArrowsPressed[0]) {
                    update.Controls.AddLast(new Control(ArrowsPressed[0], true));
                    LastArrowPressed = ArrowsPressed[0];
                }
            } else {
                if (LastArrowPressed is GAME_CONTROLS control) {
                    update.Controls.AddLast(new Control(control, false));
                    LastArrowPressed = null;
                }
            }
            // 2) Space:
            if (LastSpacePressedAt != Space.At) {
                update.Controls.AddLast(new Control(GAME_CONTROLS.SPACE, true));
                LastSpacePressedAt = Space.At;
            }
            if (update.Controls.Count > 0) await VM.SendGameUpdate(update);
        });
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    private async Task Render() { try { if (VM.IsWatching) await Canvas.Render(); } catch {} }

    // Game loop --------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task JS_OnAnimationFrame() {
        await Update();
        await Control();
        await Render();
    }
}
