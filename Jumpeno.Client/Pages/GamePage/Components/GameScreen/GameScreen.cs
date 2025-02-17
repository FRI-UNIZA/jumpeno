namespace Jumpeno.Client.Components;

public partial class GameScreen : IAsyncDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme Theme { get; set; }
    [Parameter]
    public required GameViewModel VM { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly DotNetObjectReference<GameScreen> Ref;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameScreen() => Ref = DotNetObjectReference.Create(this);

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender) return;
        if (VM.IsWatching) {
            await VM.AddAfterUpdatesListener(AfterUpdates);
        }
        if (VM.IsPlayer) {
            await VM.StartPing();
            await Window.AddKeyDownEventListener(Ref, JS_OnKeyDown);
            await Window.AddKeyUpEventListener(Ref, JS_OnKeyUp);
            await Window.AddMouseUpEventListener(Ref, JS_OnMouseUp);
        }
        await Window.AddResizeEventListener(Ref, JS_OnWindowResize);
        await Animator.AddAnimator(Ref, JS_OnAnimationFrame);
        var size = Window.GetSize();
        UpdateDimensions((int) size.Width, (int) size.Height);
        await VM.InitOnRender();
    }

    public async ValueTask DisposeAsync() {
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
            await Window.RemoveResizeEventListener(Ref, JS_OnWindowResize);
            await Animator.RemoveAnimator(Ref, JS_OnAnimationFrame);
        }
        ControlLock.Dispose();
        MouseReleaseKeyEventLock.Dispose();
        RenderLock.Dispose();
        Ref.Dispose();
        GC.SuppressFinalize(this);
    }

    // Screen -----------------------------------------------------------------------------------------------------------------------------
    // Computed screen dimensions:
    private readonly GameScreenDimension Dimension = new();

    // Screen class:
    private CSSClass GameScreenClass() {
        var c = new CSSClass("game-screen");
        if (VM.IsHost) c.Set("host");
        if (VM.IsWatching) c.Set("watching");
        if (VM.IsPlayer) c.Set("player");
        return c;
    }

    // CSS variables:
    private CSSStyle CSSVariables() {
        var s = Dimension.CSSVariables();
        s.Set("--canvas-background-color", VM.Game.Map.Background);
        return s;
    }

    // Update screen dimensions:
    private void UpdateDimensions(int width, int height) {
        Dimension.Update(width, height);
        // NOTE: [marginal-gap] + 1px to hide marginal gap when canvas is rasterized bigger than map!
        VM?.Game.Map.UpdateScreen(0, Dimension.CanvasWidth + 1, Dimension.CanvasHeight + 1, 0);
        StateHasChanged();
    }
    
    // Window resize event:
    [JSInvokable]
    public async Task JS_OnWindowResize(WindowResizeEvent e) {
        await RenderLock.TryExclusive(() => UpdateDimensions((int) e.Width, (int) e.Height));
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
    // Actions:
    private static async Task Pause() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_PAUSE, body: Game.DEFAULT_CODE));
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
    private Func<Task> TouchKeyEvent(GAME_CONTROLS control) => async () => await PressKey(control);
    private Func<Task> MouseTouchKeyEvent(GAME_CONTROLS control) => async () => await MouseReleaseKeyEventLock.TryExclusive(async () => {
        await PressKey(control);
        MouseReleaseKeyEvent = async () => await MouseReleaseKeyEventLock.TryExclusive(async () => {
            await ReleaseKey(control);
            MouseReleaseKeyEvent = () => Task.CompletedTask;
        });
    });
    [JSInvokable]
    public async Task JS_OnKeyDown(string key) {
        if (GameControlsExtension.Get(key) is not GAME_CONTROLS control) return;
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
    private Func<Task> ReleaseKeyEvent(GAME_CONTROLS control) => async () => await ReleaseKey(control);
    private Func<Task> MouseReleaseKeyEvent = () => Task.CompletedTask;
    private readonly LockerSlim MouseReleaseKeyEventLock = new();
    [JSInvokable]
    public async Task JS_OnKeyUp(string key) {
        if (GameControlsExtension.Get(key) is not GAME_CONTROLS control) return;
        await ReleaseKey(control);
    }
    [JSInvokable]
    public async Task JS_OnMouseUp((int X, int Y) position) => await MouseReleaseKeyEvent();

    // Check pressed key:
    private bool IsPressed(GAME_CONTROLS control) {
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
    private async Task<bool> IsPressedAsync(GAME_CONTROLS control) {
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
    private BECanvasComponent? GameCanvasRef = null;
    private readonly LockerSlim RenderLock = new();

    private async Task Render() {
        if (!VM.IsWatching) return;
        using var ctx = await GameCanvasRef.CreateCanvas2DAsync();
        if (ctx == null) return;
        await RenderLock.TryExclusive(async () => await VM.Game.Render(ctx, (VM.Player, Theme.FONT_PRIMARY)));
    }

    // Game loop --------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task JS_OnAnimationFrame() {
        await Update();
        await Control();
        await Render();
    }
}
