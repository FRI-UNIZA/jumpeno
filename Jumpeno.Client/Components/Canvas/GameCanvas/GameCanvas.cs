namespace Jumpeno.Client.Components;

public partial class GameCanvas {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "game-canvas";
    // Map:
    public Map DEFAULT_MAP => new(
        Map.DEFAULT_NAME, [],
        AppTheme.GAME_CANVAS_DEFAULT_BACKGROUND,
        IMAGE.MAP_JUMPERS_HOME_TILE,
        AppTheme.GAME_CANVAS_DEFAULT_FOREGROUND,
        AppTheme.GAME_CANVAS_DEFAULT_TINT,
        AppTheme.GAME_CANVAS_DEFAULT_BORDER
    );

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    /// <summary>Use for static map rendering</summary>
    [Parameter] public required Map? Map { get; set; }

    /// <summary>Use for game rendering</summary>
    [Parameter] public Game? Game { get; set; }

    /// <summary>Current game player</summary>
    [Parameter] public Player? Player { get; set; }

    /// <summary>Use for game UI or notifications</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Map --------------------------------------------------------------------------------------------------------------------------------
    public Map CurrentMap() => Map ?? Game?.Map ?? DEFAULT_MAP;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string ID = IDGenerator.Generate(nameof(GameCanvas));
    public string Selector => $"#{ID} canvas";
    public double DPR { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    // Views ------------------------------------------------------------------------------------------------------------------------------
    private readonly DotNetObjectReference<GameCanvas> Ref;
    private BECanvasComponent CanvasRef = null!;
    private readonly LockerSlim RenderLock = new();
    private Canvas2DContext? ctx = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    public CSSStyle ComputeStyle() {
        var map = CurrentMap(); return new CSSStyle()
        .Set("--canvas-background", map.Background)
        .Set("--canvas-foreground", map.Foreground)
        .Set("--canvas-border-color", map.Border)
        .Set("--canvas-box-shadow-color", map.BoxShadow.ToStringContent());
    }

    // Visibility -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task JS_OnVisibilityChange(WindowVisibilityEvent e) {
        if (!e.Hidden) await Render();
    }

    // Dimensions -------------------------------------------------------------------------------------------------------------------------
    public async Task UpdateDimensions() {
        await RenderLock.TryExclusive(() => {
            var size = Window.GetSizeOf(Selector);
            if (size == null) return;
            DPR = JS.Eval<double>("window.devicePixelRatio || 1");
            Width = (int)(size.Width * DPR);
            Height = (int)(size.Height * DPR);
        });
    }

    [JSInvokable]
    public async Task JS_OnWindowResize(WindowResizeEvent e) => await UpdateDimensions();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameCanvas() => Ref = DotNetObjectReference.Create(this);

    protected override async Task OnComponentAfterRenderAsync(bool firstRender) {
        try {
            if (!firstRender) return;
            await Window.AddVisibilityChangeEventListener(Ref, JS_OnVisibilityChange);
            await Window.AddResizeEventListener(Ref, JS_OnWindowResize);
            await UpdateDimensions();
        } finally {
            if (Game == null) await RenderMap();
        }
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        await Window.RemoveVisibilityChangeEventListener(Ref, JS_OnVisibilityChange);
        await Window.RemoveResizeEventListener(Ref, JS_OnWindowResize);
        await RenderLock.DisposeSafe();
        ctx?.Dispose();
        Ref.Dispose();
    }

    // Render -----------------------------------------------------------------------------------------------------------------------------
    private async Task RenderMap() {
        await RenderLock.TryExclusive(async () => {
            // 1) Update map:
            var map = CurrentMap(); map.UpdateScreen(0, Width, Height, 0, DPR);
            // 2) Get context:
            ctx ??= await CanvasRef.CreateCanvas2DAsync();
            // 3) Render map:
            await map.Render(ctx);
        });
    }

    private async Task RenderGame() {
        await RenderLock.TryExclusive(async () => {
            // 0) Check game:
            if (Game == null) return;
            // 1) Update map:
            Game.Map.UpdateScreen(0, Width, Height, 0, DPR);
            // 2) Get context:
            ctx ??= await CanvasRef.CreateCanvas2DAsync();
            // 3) Render game:
            await Game.Render(ctx, (Player, AppTheme.FONT_PRIMARY));
        });
    }

    public async Task Render() {
        try {
            if (Game == null) await RenderMap();
            else await RenderGame();
        } catch {}
    }
}
