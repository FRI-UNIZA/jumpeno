namespace Jumpeno.Shared.Models;

public class Map : IRectFQuadStorable, IRenderableParametric<Game> {
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

    // Background -------------------------------------------------------------------------------------------------------------------------
    public string Background { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Map(
        float worldMinX, float worldMaxX, float worldMinY, float worldMaxY,
        int screenMinX, int screenMaxX, int screenMinY, int screenMaxY,
        List<Tile> tiles,
        string background
    ) {
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
        Background = background;
    }

    public Map(float minX, float maxX, float minY, float maxY, List<Tile> tiles, string background)
    : this(minX, maxX, minY, maxY, 0, 0, 0, 0, tiles, background) {}

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private QuadTreeRectF<Tile> InitTileQT(List<Tile> tiles) {
        QuadTreeRectF<Tile> tileQT = new(Rect);
        foreach (var tile in tiles) tileQT.Add(tile);
        return tileQT;
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public void UpdateScreen(int minX, int maxX, int minY, int maxY) {
        ScreenMinX = minX;
        ScreenMaxX = maxX;
        ScreenMinY = minY;
        ScreenMaxY = maxY;
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

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public List<Tile> GetCollidingTiles(RectangleF rect) {
        return TileQT.GetObjects(rect);
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task Render(Canvas2DContext ctx, Game game) {
        var screen = ScreenRect;
        await ctx.SetFillStyleAsync($"rgb({Background})");
        await ctx.FillRectAsync(screen.X, screen.Y, screen.Width, screen.Height);
        foreach (var tile in Tiles) await tile.Render(ctx, game);
    }
}
