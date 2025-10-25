namespace Jumpeno.Client.Models;

public class Map : IRectFQuadStorable, IUpdateable, IPreRendered {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const float WIDTH = 1024;
    public const float HEIGHT = 576;
    // Color:
    public const int BOX_SHADOW_CONTRAST = 30;
    // Default:
    public const string DEFAULT_NAME = "Jumper's home";
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
    [JsonInclude][Newtonsoft.Json.JsonProperty]
    private List<Tile> Tiles { get; set; }
    private readonly QuadTreeRectF<Tile> TileQT;
    public List<Tile> GetCollidingTiles(RectangleF rect) => TileQT.GetObjects(rect);
    [JsonInclude][Newtonsoft.Json.JsonProperty]
    private string BackgroundTileImagePath { get; set; } = string.Empty;
    [JsonInclude][Newtonsoft.Json.JsonProperty]
    public string ActiveTileImagePath { get; private set; } = string.Empty;

    // Shrink -----------------------------------------------------------------------------------------------------------------------------
    public Shrink Shrink { get; private set; }

    // Colors -----------------------------------------------------------------------------------------------------------------------------
    public RGBColor Background { get; private set; }
    public RGBColor Foreground { get; private set; }
    public RGBColor Border { get; private set; }
    public RGBColor BoxShadow { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    private Map(
        string name,
        float worldMinX, float worldMaxX, float worldMinY, float worldMaxY,
        int screenMinX, int screenMaxX, int screenMinY, int screenMaxY,
        List<Tile> tiles, Shrink shrink,
        RGBColor background, RGBColor foreground, RGBColor border,
        string backgroundTileImagePath
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
        BackgroundTileImagePath = backgroundTileImagePath;
        Tiles = tiles;
        TileQT = InitTileQT(Tiles);
        Shrink = shrink;
        Background = background;
        Foreground = foreground;
        Border = border;
        BoxShadow = InitBoxShadow(border);
        PreRenderer = InitPrerenderer();
    }

    private Map(
        string name, float minX, float maxX, float minY, float maxY, List<Tile> tiles,
        RGBColor background, RGBColor foreground, RGBColor border, string backgroundTileImagePath)
    : this(name, minX, maxX, minY, maxY, 0, 0, 0, 0, tiles, null!, background, foreground, border, backgroundTileImagePath) {
        Shrink = new(this);
    }

    public Map(string name, List<Tile> tiles, RGBColor background, RGBColor foreground, RGBColor border, 
        string tileImagePath, string backgroundTileImagePath)
    : this(name, 0, WIDTH, 0, HEIGHT, tiles, background, foreground, border, backgroundTileImagePath) {
        ActiveTileImagePath = tileImagePath;
    }

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    private QuadTreeRectF<Tile> InitTileQT(List<Tile> tiles) {
        QuadTreeRectF<Tile> tileQT = new(Rect);
        foreach (var tile in tiles) tileQT.Add(tile);
        return tileQT;
    }

    private static RGBColor InitBoxShadow(RGBColor border) {
        var luma = (byte)Math.Max(border.Luminance() - BOX_SHADOW_CONTRAST, 0);
        return new(luma, luma, luma);
    }

    // Screen -----------------------------------------------------------------------------------------------------------------------------
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

    private bool TimeFlowUpdate(TimeFlowUpdate update) => Shrink.Update(update);

    private bool StateUpdate(StateUpdate update) => Shrink.Update(update);

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
    private readonly PreRenderer<Map> PreRenderer;
    private static PreRenderer<Map> InitPrerenderer() => new(CANVAS.MAP, PreRender, ApplyRender);
    private static async Task<bool> PreRender(Canvas2DContext ctx, Map map) {
        // 1) Initialize:
        var prerendered = false;
        // 2) Clear:
        await ctx.ClearRectAsync(map.WorldMinX, map.WorldMinY, map.WorldWidth, map.WorldHeight);
        // 3) Render tiles:
        for (int i = 0; i < map.Tiles.Count; i++) {
            if (!await map.Tiles[i].Render(ctx, (map, false))) break;
            if (i > 0) continue;
            prerendered = true;
        }
        // 4) Return result:
        return prerendered;
    }
    private static async Task<bool> ApplyRender((Canvas2DContext Source, Canvas2DContext Destination) context, Map map) {
        var (source, ctx) = context;
        var world = map.Rect;
        var screen = map.ScreenRect;
        await ctx.DrawImageAsync(
            source.Canvas, world.X, world.Y, world.Width, world.Height,
            screen.X, screen.Y, screen.Width, screen.Height
        );
        return true;
    }
    public bool IsPrerendered => PreRenderer.IsPrerendered;
    public async Task<bool> PreRender() => await PreRenderer.PreRender(this);

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, Game? game = null) {
        var screen = ScreenRect;
        // 1) Background:
        if (ImageReferrer.Get(BackgroundTileImagePath) is ElementReference img)
        {
            var pattern = await ctx.CreatePatternAsync(img, RepeatPattern.Repeat);
            await ctx.SetFillStyleAsync(pattern);
        }
        else
        {
            await ctx.SetFillStyleAsync($"{Background}");
        }
        await ctx.FillRectAsync(screen.X, screen.Y, screen.Width, screen.Height);
        // 2) Shrink:
        if (game != null) await Shrink.Render(ctx, game);
        // 3) Tiles:
        await PreRenderer.Render(ctx, this);
        return true;
    }
}
