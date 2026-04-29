namespace Jumpeno.Client.Components;

public partial class GameScreen {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly DotNetObjectReference<GameScreen> Ref;
    private GameCanvas Canvas = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set("game-screen", Base).Set(VM.CSSClass());

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
    private readonly List<GameControls> ArrowsPressed = [];
    private GameControls? LastArrowPressed = null;
    // Space:
    private (bool Pressed, DateTime? At) Space = (false, null);
    private DateTime? LastSpacePressedAt = null;
    // Lock:
    private readonly LockerSlim ControlLock = new();

    // Display controls:
    private CssClass GameControlsClass() {
        var c = new CssClass("game-controls");
        if (!VM.ControlsDisplayed) c.Set("hidden");
        return c;
    }
    private CssClass ControlClass(GameControls control) {
        var c = new CssClass("control");
        switch (control) {
            case GameControls.Space: c.Set("space"); break;
            case GameControls.Left: c.Set("left"); break;
            case GameControls.Right: c.Set("right"); break;
        }
        if (IsPressed(control)) c.Set("pressed");
        return c;
    }

    // Save pressed keys:
    private async Task PressKey(GameControls control) {
        await ControlLock.TryExclusive(() => {
            switch (control) {
                case GameControls.Space:
                    if (Space.Pressed) break;
                    Space = (true, DateTime.UtcNow);
                break;
                case GameControls.Left:
                case GameControls.Right:
                    if (ArrowsPressed.Contains(control)) break;
                    ArrowsPressed.Add(control);
                break;
            }
        });
    }
    private Func<Task> TouchKeyEvent(GameControls control) => () => PressKey(control);
    private Func<MouseEventArgs, Task> MouseTouchKeyEvent(GameControls control)
    => e => MouseReleaseKeyEventLock.TryExclusive(async () => {
        if (e.Button != MouseButton.Left.Raw()) return;
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
        if (GameControlsExtension.Get(e.Key) is not GameControls control) return;
        await PressKey(control);
    }

    // Save released keys:
    private async Task ReleaseKey(GameControls control) {
        await ControlLock.TryExclusive(() => {
            switch (control) {
                case GameControls.Space:
                    Space = (false, Space.At);
                break;
                case GameControls.Left:
                case GameControls.Right:
                    ArrowsPressed.Remove(control);
                break;
            }
        });
    }
    private Func<Task> ReleaseKeyEvent(GameControls control) => () => ReleaseKey(control);
    private Func<Task> MouseReleaseKeyEvent = () => Task.CompletedTask;
    private readonly LockerSlim MouseReleaseKeyEventLock = new();
    [JSInvokable]
    public async Task JS_OnKeyUp(WindowKeyEvent e) {
        if (e.Repeat) return;
        if (GameControlsExtension.Get(e.Key) is not GameControls control) return;
        await ReleaseKey(control);
    }
    [JSInvokable]
    public async Task JS_OnMouseUp(WindowMouseEvent e) {
        if (e.Button == MouseButton.Left) await MouseReleaseKeyEvent();
    }

    // Check pressed key:
    protected bool IsPressed(GameControls control) {
        switch (control) {
            case GameControls.Space:
                return Space.Pressed;
            case GameControls.Left:
            case GameControls.Right:
                return ArrowsPressed.Contains(control);
            default:
                return false;
        }
    }
    protected async Task<bool> IsPressedAsync(GameControls control) {
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
                if (LastArrowPressed is GameControls control) {
                    update.Controls.AddLast(new Control(control, false));
                    LastArrowPressed = null;
                }
            }
            // 2) Space:
            if (LastSpacePressedAt != Space.At) {
                update.Controls.AddLast(new Control(GameControls.Space, true));
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
