namespace Jumpeno.Shared.Models;

public class Shrink : IUpdateable, IPreRendered<Game> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static (int Level, double Timer) DEFAULT => (-1, DURATION_MS);

    public static readonly RGBColor COLOR = new(172, 192, 193);
    public const double MIN_ALPHA = 0.2;
    public const double MAX_ALPHA = 0.5;

    public const double FADE_IN_MS = 400; // ms
    public const double MARK_MS = 3000; // ms
    public const double HIGHLIGHT_MS = 2000; // ms
    public const double DURATION_MS = MARK_MS + HIGHLIGHT_MS; // ms
    public const double BLICK_INTERVAL_MS = 800; // ms
    public const double HALF_BLICK_INTERVAL_MS = BLICK_INTERVAL_MS / 2; // ms

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int Level { get; private set; }
    public int MaxLevel { get; private set; }
    public double Timer { get; private set; }
    public double Alpha { get {
        var t = Math.Max(Timer - MARK_MS, 0) % BLICK_INTERVAL_MS;
        var alpha = Level == 0 && Timer < FADE_IN_MS ? MIN_ALPHA * (Timer % FADE_IN_MS / FADE_IN_MS) : MIN_ALPHA;
        alpha += (MAX_ALPHA - MIN_ALPHA) * (1 - Math.Abs(HALF_BLICK_INTERVAL_MS - t) / HALF_BLICK_INTERVAL_MS);
        return alpha;
    } }

    [JsonInclude] private float WorldX { get; set; }
    [JsonInclude] private float WorldY { get; set; }
    [JsonInclude] private float WorldWidth { get; set; }
    [JsonInclude] private float WorldHeight { get; set; }
    public RectangleF Rect => new(
        WorldX + Math.Max(Level, 0) * Tile.SIZE, WorldY,
        WorldWidth - 2 * Math.Max(Level, 0) * Tile.SIZE, WorldHeight
    );

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public Shrink(int level, int maxLevel, double timer, float worldX, float worldY, float worldWidth, float worldHeight) {
        // Properties:
        Level = level;
        MaxLevel = maxLevel;
        Timer = timer;
        // World dimensions:
        WorldX = worldX;
        WorldY = worldY;
        WorldWidth = worldWidth;
        WorldHeight = worldHeight;
        // Prerender:
        PreRenderer = InitPrerenderer();
    }

    public Shrink(Map map) : this(DEFAULT.Level, InitMaxLevel(map), DEFAULT.Timer, 0, 0, 0, 0) {
        var rect = map.Rect;
        WorldX = rect.X;
        WorldY = rect.Y;
        WorldWidth = rect.Width;
        WorldHeight = rect.Height;
    }

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private static int InitMaxLevel(Map map) {
        var steps = (int) map.WorldWidth / Tile.SIZE;
        return steps / 2 + steps % 2;
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update) {
        if (update is TimeFlowUpdate time) return TimeFlowUpdate(time);
        if (update is StateUpdate state) return StateUpdate(state);
        return false;
    }

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        if (update.Game.State != GAME_STATE.SHRINKING) return false;
        Timer += update.DeltaT;
        return true;
    }

    private bool StateUpdate(StateUpdate update) {
        Level = update.Level;
        Timer = update.Timer;
        return true;
    }

    // Pre-Rendering ----------------------------------------------------------------------------------------------------------------------
    private readonly PreRenderer<Game> PreRenderer;
    private PreRenderer<Game> InitPrerenderer() => new(CANVAS.TILE_PATTERN, PreRender, ApplyRender);
    private async Task<bool> PreRender(Canvas2DContext ctx, Game game) {
        // 1) Initialize:
        var prerendered = false;
        // 2) Clear:
        await ctx.ClearRectAsync(WorldX, WorldY, WorldWidth, WorldHeight);
        // 3) Render pattern:
        for (int i = 0, x = (int) WorldX; x < WorldX + WorldWidth; x += Tile.SIZE) {
            for (float y = WorldY; y < WorldY + WorldHeight; y += Tile.SIZE, i++) {
                var tile = new Tile(new(x + Tile.HALF_SIZE, y + Tile.HALF_SIZE));
                if (!await tile.Render(ctx, (game, false))) break;
                if (i > 0) continue;
                prerendered = true;
            }
        }
        // 4) Return result:
        return prerendered;
    }
    private async Task<bool> ApplyRender((Canvas2DContext Source, Canvas2DContext Destination) context, Game game) {
        // 1) Check state & init:
        if (game.State != GAME_STATE.SHRINKING || Level < 0) return false;
        var (source, ctx) = context; var rect = Rect;

        // 2.1) Highlight area color & size:
        await ctx.SetFillStyleAsync($"rgb({COLOR.Blend(Alpha, game.Map.Background)})");
        var add = rect.Width < 2 * Tile.SIZE + Tile.HALF_SIZE ? 4 : 1;
        var size = new Size(game.Map.ToScreenWidth(Tile.SIZE + Tile.HALF_SIZE) + add, game.Map.ToScreenHeight(rect.Height) + 1);
        // 2.2) Left part:
        var point = game.Map.ToScreen(new(rect.X - Tile.HALF_SIZE, rect.Y + rect.Height));
        await ctx.FillRectAsync(point.X - 0.5, point.Y - 0.5, size.Width, size.Height);
        // 2.3) Right part:
        point = game.Map.ToScreen(new(rect.X + rect.Width - Tile.SIZE, rect.Y + rect.Height));
        await ctx.FillRectAsync(point.X - 0.5, point.Y - 0.5, size.Width, size.Height);

        // 3.1) Tiles size:
        if (Level <= 0) return true;
        var sizeF = new SizeF(rect.X - WorldX, rect.Height); var screen = game.Map.ScreenRect;
        size = new(game.Map.ToScreenWidth(sizeF.Width), game.Map.ToScreenHeight(sizeF.Height));
        // 3.2) Left part:
        await ctx.DrawImageAsync(
            source.Canvas, WorldX, WorldY, sizeF.Width, sizeF.Height,
            screen.X - 0.5, screen.Y - 0.5, size.Width + 2.5, screen.Height + 1
        );
        // 3.3) Right part:
        await ctx.DrawImageAsync(
            source.Canvas, WorldX, WorldY, sizeF.Width, sizeF.Height,
            screen.X + screen.Width - size.Width - 2, screen.Y - 0.5, size.Width + 2.5, screen.Height + 1
        ); return true;
    }
    public bool IsPrerendered => PreRenderer.IsPrerendered;
    public async Task<bool> PreRender(Game game) => await PreRenderer.PreRender(game);

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, Game game) {
        return await PreRenderer.Render(ctx, game);
    }
}
