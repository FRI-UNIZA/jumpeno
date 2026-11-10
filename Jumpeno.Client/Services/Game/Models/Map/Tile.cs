namespace Jumpeno.Client.Models;

public class Tile : IRectFPositionable, IRenderable<(Map Map, bool Scale)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int Size = 64;
    public const int HalfSize = Size / 2;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    [JsonInclude][Newtonsoft.Json.JsonProperty]
    private PointF Center => Position.Center;
    public RectFPosition Position { get; private set; }
    public RectangleF Rect { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    public Tile(PointF center) {
        Position = new(center, Size - 2, Size - 2);
        Rect = Collision.GetBoundingBox(Position);
    }

    public Tile(int x, int y) : this(new PointF(x * Size + HalfSize, y * Size + HalfSize)) {}

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
        int size = scale ? map.ToScreenWidth(Size) : Size;
        if (ImageReferrer.Get(map.TileImage) is not ElementReference img) return false;
        await ctx.DrawImageAsync(
            img,
            0, 0,
            Size, Size,
            point.X - size / 2 - 0.5, point.Y - size / 2 - 0.5,
            size + 1, size + 1
        );
        return true;
    }
}
