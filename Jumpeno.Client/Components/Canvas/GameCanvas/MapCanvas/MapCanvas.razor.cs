namespace Jumpeno.Client.Components;

public partial class MapCanvas {
    // Identifiers ------------------------------------------------------------------------------------------------------------------------
    public override bool Dynamic => false;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter] public required Map? Map { get; set; }

    // Map --------------------------------------------------------------------------------------------------------------------------------
    private Map DEFAULT_MAP => new(
        Map.DEFAULT_NAME, [],
        AppTheme.GAME_CANVAS_DEFAULT_BACKGROUND,
        ImageType.MAP_JUMPERS_HOME_TILE,
        AppTheme.GAME_CANVAS_DEFAULT_FOREGROUND,
        AppTheme.GAME_CANVAS_DEFAULT_TINT,
        AppTheme.GAME_CANVAS_DEFAULT_BORDER
    );

    private Map UpdateMap() {
        var map = CurrentMap();
        map.UpdateScreen(0, Width, Height, 0, DPR);
        return map;
    }

    protected override Map CurrentMap() => Map ?? DEFAULT_MAP;

    // Render -----------------------------------------------------------------------------------------------------------------------------
    protected override async Task PreRenderCanvas() {
        // 1) Update map:
        var map = UpdateMap();
        // 2) Pre-Render map:
        await map.PreRender();
    }

    protected override async Task RenderCanvas() {
        // 1) Update map:
        var map = UpdateMap();
        // 2) Get context:
        ctx ??= await CanvasRef.CreateCanvas2DAsync();
        // 3) Render map:
        await map.Render(ctx);
    }
}
