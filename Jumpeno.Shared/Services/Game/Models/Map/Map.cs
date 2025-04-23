namespace Jumpeno.Shared.Models;

public class Map : IRectFQuadStorable, IUpdateable, IPreRendered<Game> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string DEFAULT_NAME = "Jumper's home";
    public const float WIDTH = 1024;
    public const float HEIGHT = 576;
    public static readonly RGBColor DEFAULT_BACKGROUND = new(36, 30, 59);
    public static readonly RGBColor DEFAULT_FOREGROUND = new(255, 255, 0);
    public static List<Tile> DEFAULT_TILES => [
        new(new(1 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(2 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(5 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(6 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(7 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(8 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(9 * Tile.SIZE + Tile.HALF_SIZE, 2 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(10 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 1 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 7 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(12 * Tile.SIZE + Tile.HALF_SIZE, 8 * Tile.SIZE + Tile.HALF_SIZE)),
        new(new(13 * Tile.SIZE + Tile.HALF_SIZE, 0 * Tile.SIZE + Tile.HALF_SIZE))
    ];
    public static readonly Map DEFAULT_MAP = new(DEFAULT_NAME, DEFAULT_TILES, DEFAULT_BACKGROUND, DEFAULT_FOREGROUND);

    // Description ------------------------------------------------------------------------------------------------------------------------
    public string Name { get; private set; }

    // World ------------------------------------------------------------------------------------------------------------------------------
    public float WorldMinX { get; private set; }
    public float WorldMaxX { get; private set; }
    public float WorldMinY { get; private set; }
    public float WorldMaxY { get; private set; }
    public float WorldWidth => Math.Abs(WorldMaxX - WorldMinX);
    public float WorldHeight => Math.Abs(WorldMaxY - WorldMinY);
    public RectangleF Rect => new(
        Math.Min(WorldMinX, WorldMaxX), Math.Min(WorldMinY, WorldMaxY),
        WorldWidth, WorldHeight
    );

    // Screen -----------------------------------------------------------------------------------------------------------------------------
    public int ScreenMinX { get; private set; } = 0;
    public int ScreenMaxX { get; private set; } = 0;
    public int ScreenMinY { get; private set; } = 0;
    public int ScreenMaxY { get; private set; } = 0;
    public int ScreenWidth => Math.Abs(ScreenMaxX - ScreenMinX);
    public int ScreenHeight => Math.Abs(ScreenMaxY - ScreenMinY);
    public RectangleF ScreenRect => new(
        Math.Min(ScreenMinX, ScreenMaxX), Math.Min(ScreenMinY, ScreenMaxY),
        ScreenWidth, ScreenHeight
    );

    // Tiles ------------------------------------------------------------------------------------------------------------------------------
    [JsonInclude]
    private List<Tile> Tiles { get; set; }
    private readonly QuadTreeRectF<Tile> TileQT;
    public List<Tile> GetCollidingTiles(RectangleF rect) => TileQT.GetObjects(rect);

    // Shrink -----------------------------------------------------------------------------------------------------------------------------
    public Shrink Shrink { get; private set; }

    // Colors -----------------------------------------------------------------------------------------------------------------------------
    public RGBColor Background { get; private set; }
    public RGBColor Foreground { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Map(
        string name,
        float worldMinX, float worldMaxX, float worldMinY, float worldMaxY,
        int screenMinX, int screenMaxX, int screenMinY, int screenMaxY,
        List<Tile> tiles, Shrink shrink,
        RGBColor background, RGBColor foreground
    ) {
        Name = name;
        WorldMinX = worldMinX;
        WorldMaxX = worldMaxX;
        WorldMinY = worldMinY;
        WorldMaxY = worldMaxY;
        ScreenMinX = screenMinX;
        ScreenMaxX = screenMaxX;
        ScreenMinY = screenMinY;
        ScreenMaxY = screenMaxY;
        Tiles = tiles;
        TileQT = InitTileQT(Tiles);
        Shrink = shrink;
        Background = background;
        Foreground = foreground;
        PreRenderer = InitPrerenderer();
    }

    private Map(string name, float minX, float maxX, float minY, float maxY, List<Tile> tiles, RGBColor background, RGBColor foreground)
    : this(name, minX, maxX, minY, maxY, 0, 0, 0, 0, tiles, null!, background, foreground) {
        Shrink = new(this);
    }

    public Map(string name, List<Tile> tiles, RGBColor background, RGBColor foreground)
    : this(name, 0, WIDTH, 0, HEIGHT, tiles, background, foreground) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private QuadTreeRectF<Tile> InitTileQT(List<Tile> tiles) {
        QuadTreeRectF<Tile> tileQT = new(Rect);
        foreach (var tile in tiles) tileQT.Add(tile);
        return tileQT;
    }

    public void UpdateScreen(int minX, int maxX, int minY, int maxY) {
        ScreenMinX = minX;
        ScreenMaxX = maxX;
        ScreenMinY = minY;
        ScreenMaxY = maxY;
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update) {
        if (update is TimeFlowUpdate time) return TimeFlowUpdate(time);
        if (update is StateUpdate state) return StateUpdate(state);
        return false;
    }

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        return Shrink.Update(update);
    }

    private bool StateUpdate(StateUpdate update) {
        return Shrink.Update(update);
    }

    // Conversions ------------------------------------------------------------------------------------------------------------------------
    public Point ToScreen(PointF point) {
        return new Point(
            (int) ((point.X - WorldMinX) / (WorldMaxX - WorldMinX) * (ScreenMaxX - ScreenMinX) + ScreenMinX),
            (int) ((point.Y - WorldMinY) / (WorldMaxY - WorldMinY) * (ScreenMaxY - ScreenMinY) + ScreenMinY)
        );
    }
    public int ToScreenWidth(float width) => (int) Math.Abs(width / (WorldMaxX - WorldMinX) * (ScreenMaxX - ScreenMinX));
    public int ToScreenHeight(float height) => (int) Math.Abs(height / (WorldMaxY - WorldMinY) * (ScreenMaxY - ScreenMinY));

    public PointF ToWorld(Point point) {
        return new PointF(
            (point.X - ScreenMinX) / (float) (ScreenMaxX - ScreenMinX) * (WorldMaxX - WorldMinX) + WorldMinX,
            (point.Y - ScreenMinY) / (float) (ScreenMaxY - ScreenMinY) * (WorldMaxY - WorldMinY) + WorldMinY
        );
    }
    public float ToWorldWidth(int width) => Math.Abs(width / (float) (ScreenMaxX - ScreenMinX) * (WorldMaxX - WorldMinX));
    public float ToWorldHeight(int height) => Math.Abs(height / (float) (ScreenMaxY - ScreenMinY) * (WorldMaxY - WorldMinY));

    public Point ToCanvas(PointF point) => new((int) point.X, (int) (WorldHeight - point.Y));

    // Pre-Rendering ----------------------------------------------------------------------------------------------------------------------
    private readonly PreRenderer<Game> PreRenderer;
    private PreRenderer<Game> InitPrerenderer() => new(CANVAS.MAP, PreRender, ApplyRender);
    private async Task<bool> PreRender(Canvas2DContext ctx, Game game) {
        // 1) Initialize:
        var prerendered = false;
        // 2) Clear:
        await ctx.ClearRectAsync(WorldMinX, WorldMinY, WorldWidth, WorldHeight);
        // 3) Render tiles:
        for (int i = 0; i < Tiles.Count; i++) {
            if (!await Tiles[i].Render(ctx, (game, false))) break;
            if (i > 0) continue;
            prerendered = true;
        }
        // 4) Return result:
        return prerendered;
    }
    private async Task<bool> ApplyRender((Canvas2DContext Source, Canvas2DContext Destination) context, Game game) {
        var (source, ctx) = context;
        var world = Rect;
        var screen = ScreenRect;
        await ctx.DrawImageAsync(
            source.Canvas, world.X, world.Y, world.Width, world.Height,
            screen.X, screen.Y, screen.Width, screen.Height
        );
        return true;
    }
    public bool IsPrerendered => PreRenderer.IsPrerendered;
    public async Task<bool> PreRender(Game game) => await PreRenderer.PreRender(game);

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, Game game) {
        var screen = ScreenRect;
        // 1) Background:
        await ctx.SetFillStyleAsync($"rgb({Background})");
        await ctx.FillRectAsync(screen.X, screen.Y, screen.Width, screen.Height);
        // 2) Shrink:
        await Shrink.Render(ctx, game);
        // 3) Tiles:
        await PreRenderer.Render(ctx, game);
        return true;
    }
}
