namespace Jumpeno.Client.Components;

public partial class MapCanvas {
    // Identifiers ------------------------------------------------------------------------------------------------------------------------
    public override bool Dynamic => false;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter] public required Map? Map { get; set; }

    // Map --------------------------------------------------------------------------------------------------------------------------------
    private Map DefaultMap => new(
        Map.DefaultName, [],
        AppTheme.GameCanvasDefaultBackground,
        ImageType.MapJumpersHomeTile,
        AppTheme.GameCanvasDefaultForeground,
        AppTheme.GameCanvasDefaultTint,
        AppTheme.GameCanvasDefaultBorder
    );

    private Map UpdateMap() {
        var map = CurrentMap();
        map.UpdateScreen(0, Width, Height, 0, DPR);
        return map;
    }

    protected override Map CurrentMap() => Map ?? DefaultMap;

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
