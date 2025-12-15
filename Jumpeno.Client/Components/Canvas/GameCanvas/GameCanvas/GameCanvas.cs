namespace Jumpeno.Client.Components;

public partial class GameCanvas {
    // Identifiers ------------------------------------------------------------------------------------------------------------------------
    public override bool Dynamic => true;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter] public required Game Game { get; set; }
    [Parameter] public Player? Player { get; set; }

    // Map --------------------------------------------------------------------------------------------------------------------------------
    private void UpdateMap() => Game.Map.UpdateScreen(0, Width, Height, 0, DPR);

    protected override Map CurrentMap() => Game.Map;

    // Render -----------------------------------------------------------------------------------------------------------------------------
    protected override async Task PreRenderCanvas() {
        // 1) Update map:
        UpdateMap();
        // 2) Pre-Render map:
        await Game.Map.PreRender(Game);
    }

    protected override async Task RenderCanvas() {
        // 1) Update map:
        UpdateMap();
        // 2) Get context:
        ctx ??= await CanvasRef.CreateCanvas2DAsync();
        // 3) Render game:
        await Game.Render(ctx, (Player, AppTheme.FONT_PRIMARY));
    }
}
