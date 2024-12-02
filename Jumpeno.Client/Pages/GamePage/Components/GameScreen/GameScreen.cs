namespace Jumpeno.Client.Components;

public partial class GameScreen : IAsyncDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme Theme { get; set; }
    [Parameter]
    public required GameViewModel VM { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Reference:
    private readonly DotNetObjectReference<GameScreen> Ref;
    // Rendering:
    private BECanvasComponent? GameCanvasRef = null;
    private Canvas2DContext? GameCanvasContext = null;
    private readonly LockerSlim RenderLock = new();

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameScreen() => Ref = DotNetObjectReference.Create(this);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            await VM.StartPing();
            await VM.AddAfterUpdateListener(AfterUpdate);
            await VM.AddAfterUpdatesListener(AfterUpdates);
            await VM.InitOnRender();
            await Window.AddResizeEventListener(Ref, JS_OnWindowResize);
            await Window.AddKeyDownEventListener(Ref, JS_OnKeyDown);
            await Window.AddKeyUpEventListener(Ref, JS_OnKeyUp);
            await Animator.AddAnimator(Ref, JS_OnAnimationFrame);
            var size = Window.GetSize();
            UpdateDimensions((int) size.Width, (int) size.Height);
        } else if (GameCanvasRef != null && GameCanvasContext == null) {
            GameCanvasContext = await GameCanvasRef.CreateCanvas2DAsync();
            await Render(GameCanvasContext);
        }
    }

    public async ValueTask DisposeAsync() {
        if (!AppEnvironment.IsServer) {
            await VM.StopPing();
            await VM.RemoveAfterUpdateListener(AfterUpdate);
            await VM.RemoveAfterUpdatesListener(AfterUpdates);
            await Window.RemoveResizeEventListener(Ref, JS_OnWindowResize);
            await Window.RemoveKeyDownEventListener(Ref, JS_OnKeyDown);
            await Window.RemoveKeyUpEventListener(Ref, JS_OnKeyUp);
            await Animator.RemoveAnimator(Ref, JS_OnAnimationFrame);
            GameCanvasContext?.Dispose();
        }
        Ref.Dispose();
    }

    // Screen dimensions ------------------------------------------------------------------------------------------------------------------
    private int CalcPaddingHorizontal(int screenWidth) {
        if (screenWidth >= 1200) {
            return Theme.SIZE_CONTAINER_PADDING_DESKTOP + 40;
        } else if (screenWidth >= 768) {
            return Theme.SIZE_CONTAINER_PADDING_TABLET + 20;
        } else {
            return 0;
        }
    }

    private int CalcPaddingVertical(int screenWidth) {
        if (screenWidth >= 1200) {
            return Theme.SIZE_CONTAINER_PADDING_DESKTOP + 40 + 20;
        } else if (screenWidth >= 768) {
            return Theme.SIZE_CONTAINER_PADDING_TABLET + 20 + 20;
        } else {
            return 40;
        }
    }

    private void UpdateDimensions(int screenWidth, int screenHeight) {
        var originalWidth = screenWidth;
        screenWidth = Math.Max(screenWidth - 2 * CalcPaddingHorizontal(originalWidth), 1);
        screenHeight = Math.Max(screenHeight - 2 * CalcPaddingVertical(originalWidth), 1);
        int width;
        int height;
        if (originalWidth < 768 || (screenWidth / (double) screenHeight < 16 / 9.0)) {
            // NOTE: [marginal-gap] + 1px on mobile phones
            width = screenWidth + 1;
            height = (int) (width * 0.5625) + 1;
        } else {
            height = screenHeight;
            width = (int) (16 / 9.0 * height);
            if (height < 260) {
                width = Math.Min(462, originalWidth);
                height = (int) (width * 0.5625);
            }
        }
        VM.Game.Map.UpdateScreen(0, width, height, 0);
        StateHasChanged();
    }

    // NOTE: [marginal-gap] Canvas is rendered smaller to hide marginal gap
    private int CanvasWidth => VM.Game.Map.ScreenWidth - 1;
    private int CanvasHeight => VM.Game.Map.ScreenHeight - 1;

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    // 1) Apply server updates:
    private async Task Update() => await VM.ExecuteUpdates();
    
    // 2) After each server update:
    // NOTE: [Controls] Checks applied jump and allows press again
    public async Task AfterUpdate(UpdateAfterEvent e) {
        if (e.Update is not GamePlayUpdate update) return;
        await ControlLock.Exclusive(() => {
            update.Movements.TryGetValue(VM.Player.ID, out var movementUpdate);
            if (movementUpdate is not MovementUpdate movement) return;
            foreach (var keyUpdateID in movement.KeyUpdateIDs) {
                if (LastSpaceUpdate == null || keyUpdateID != LastSpaceUpdate.ID) continue;
                LastSpaceUpdate = null;
            }
        });
    }

    // 3) After all server updates:
    public async Task AfterUpdates() {
        var deltaT = await VM.Game.Clock.AwaitDelta();
        VM.Game.Update(VM.Game.NewTimeFlowUpdate(deltaT));
    } 

    // Controls ---------------------------------------------------------------------------------------------------------------------------
    // Arrows:
    private readonly List<GAME_CONTROLS> ArrowsPressed = [];
    private GAME_CONTROLS? LastArrowPressed = null;
    // Space:
    private bool SpacePressed = false;
    private KeyUpdate? LastSpaceUpdate = null;
    // Lock:
    private readonly LockerSlim ControlLock = new();

    // Save pressed keys:
    [JSInvokable]
    public async Task JS_OnKeyDown(string key) {
        await ControlLock.Exclusive(() => {
            var control = GameControlsExtension.Get(key);
            switch (control) {
                case GAME_CONTROLS.SPACE:
                    SpacePressed = true;
                break;
                case GAME_CONTROLS.LEFT:
                case GAME_CONTROLS.RIGHT:
                    if (ArrowsPressed.Contains((GAME_CONTROLS) control)) break;
                    ArrowsPressed.Add((GAME_CONTROLS) control);
                break;
            }
        });
    }

    // Save released keys:
    [JSInvokable]
    public async Task JS_OnKeyUp(string key) {
        await ControlLock.Exclusive(() => {
            var control = GameControlsExtension.Get(key);
            switch (control) {
                case GAME_CONTROLS.SPACE:
                    SpacePressed = false;
                break;
                case GAME_CONTROLS.LEFT:
                case GAME_CONTROLS.RIGHT:
                    ArrowsPressed.Remove((GAME_CONTROLS) control);
                break;
            }
        });
    }

    // Send pressed keys to the server:
    private async Task Control() {
        await ControlLock.Exclusive(async () => {
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
            if (SpacePressed) {
                if (LastSpaceUpdate == null && !VM.Player.IsJumping) {
                    update.Controls.AddLast(new Control(GAME_CONTROLS.SPACE, true));
                    LastSpaceUpdate = update;
                }
            }
            if (update.Controls.Count > 0) await VM.SendGameUpdate(update);
        });
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    // Handle window resize:
    [JSInvokable]
    public async Task JS_OnWindowResize(WindowResizeEvent e) {
        await RenderLock.Exclusive(() => {
            UpdateDimensions((int) e.Width, (int) e.Height);
        });
    }

    // Render game:
    private async Task Render(Canvas2DContext ctx) {
        await RenderLock.Exclusive(async () => {
            await VM.Game.Render(ctx);
        });
    }

    // Game loop --------------------------------------------------------------------------------------------------------------------------
    // Execute loop:
    private async Task GameLoop(Canvas2DContext ctx) {
        await Update();
        await Control();
        await Render(ctx);
    }

    // On each frame:
    [JSInvokable]
    public async Task JS_OnAnimationFrame() {
        if (GameCanvasContext == null) return;
        await GameLoop(GameCanvasContext);
    }
}
