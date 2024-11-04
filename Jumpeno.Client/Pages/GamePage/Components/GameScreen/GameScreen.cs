namespace Jumpeno.Client.Components;

public partial class GameScreen: IDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required BaseTheme Theme { get; set; }
    [Parameter]
    public required GameViewModel ViewModel { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly DotNetObjectReference<GameScreen> ObjRef;
    private BECanvasComponent? GameCanvasRef = null;
    private Canvas2DContext? GameCanvasContext = null;

    // Canvas dimensions (Automatically updated based on screen resolution and resize):
    private int Width = 16;
    private int Height = 9;
    private string BackgroundColor = "rgb(36, 30, 59)";

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameScreen() {
        ObjRef = DotNetObjectReference.Create(this);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender) {
            if (GameCanvasRef == null) return;
            GameCanvasContext = await GameCanvasRef.CreateCanvas2DAsync();
            await Draw(GameCanvasContext);
            return;
        }
        await ViewModel.OnRender.Invoke();
        Window.AddResizeEventListener(ObjRef, JS_OnWindowResize);
        Animator.AddAnimator(ObjRef, JS_OnAnimationFrame);
        var size = Window.GetSize();
        UpdateDimensions((int) size.Width, (int) size.Height);
    }

    public void Dispose() {
        if (!AppEnvironment.IsServer()) {
            Window.RemoveResizeEventListener(ObjRef, JS_OnWindowResize);
            Animator.RemoveAnimator(ObjRef, JS_OnAnimationFrame);
        }
        ObjRef.Dispose();
    }

    // Calculations -----------------------------------------------------------------------------------------------------------------------
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
        if (originalWidth < 768 || (screenWidth / (double) screenHeight < 16 / 9.0)) {
            Width = screenWidth;
            Height = (int) (Width * 0.5625);
        } else {
            Height = screenHeight;
            Width = (int) (16 / 9.0 * Height);
            if (Height < 260) {
                Width = Math.Min(462, originalWidth);
                Height = (int) (Width * 0.5625);
            }
        }
        ViewModel.Game.Map.UpdateScreen(0, Width, Height, 0);
        StateHasChanged();
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public void JS_OnWindowResize(WindowResizeEvent e) {
        UpdateDimensions((int) e.Width, (int) e.Height);
    }

    private DateTime LastTime = DateTime.UtcNow;
    [JSInvokable]
    public async Task JS_OnAnimationFrame() {
        var now = DateTime.UtcNow;
        var deltaT = (now - LastTime).TotalMilliseconds;
        if (deltaT <= 0) return;
        await GameLoop(deltaT);
        LastTime = now;
    }

    private async Task GameLoop(double deltaT) {
        if (GameCanvasContext == null) return;
        await Update(deltaT);
        await Draw(GameCanvasContext);
    }

    private async Task Update(double deltaT) {
        foreach (var (player, index) in ViewModel.Game.PlayerIterator) {
            player.Avatar.Update(deltaT, ViewModel.Game);
        }
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    private async Task Draw(Canvas2DContext ctx) {
        var Game = ViewModel.Game;
        var Map = Game.Map;
        await ctx.ClearRectAsync(0, 0, Width, Height);
        foreach (var (player, index) in Game.PlayerIterator) {
            await player.Avatar.Draw(ctx, Map, player?.User?.Skin);
        }
        // await ctx.SetFillStyleAsync("green");
        // await ctx.FillRectAsync(10, 100, 100, 100);
        // await ctx.SetFontAsync("48px serif");
        // await ctx.StrokeTextAsync("Hello Blazor!!!", 10, 100);
    }
}
