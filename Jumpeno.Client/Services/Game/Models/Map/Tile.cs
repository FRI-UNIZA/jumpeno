namespace Jumpeno.Client.Models;

public class Tile : IRectFPositionable, IRenderable<(Map Map, bool Scale)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int SIZE = 64;
    public const int HALF_SIZE = SIZE / 2;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    [JsonInclude][Newtonsoft.Json.JsonProperty]
    private PointF Center => Position.Center;
    public RectFPosition Position { get; private set; }
    public RectangleF Rect { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    public Tile(PointF center) {
        Position = new(center, SIZE - 2, SIZE - 2);
        Rect = Collision.GetBoundingBox(Position);
    }

    public Tile(int x, int y) : this(new PointF(x * SIZE + HALF_SIZE, y * SIZE + HALF_SIZE))
    {
    }

    // Static tile creation methods -------------------------------------------------------------------------------------------------------
    public static List<Tile> CreateTiles(List<(int x, int y)> tilePositions)
    {
        var tiles = new List<Tile>();
        foreach (var (x, y) in tilePositions)
        {
            tiles.Add(new Tile(x, y));
        }
        return tiles;
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, (Map Map, bool Scale) @params) {
        var (map, scale) = @params;
        var point = scale ? map.ToScreen(Center) : map.ToCanvas(Center);
        int size = scale ? map.ToScreenWidth(SIZE) : SIZE;
        if (ImageReferrer.Get(map.TileImage) is not ElementReference img) return false;
        await ctx.DrawImageAsync(
            img,
            0, 0,
            SIZE, SIZE,
            point.X - size / 2 - 0.5, point.Y - size / 2 - 0.5,
            size + 1, size + 1
        );
        return true;
    }
}
