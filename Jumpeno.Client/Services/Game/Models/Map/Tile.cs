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
    [JsonInclude][Newtonsoft.Json.JsonProperty]
    private string ImagePath { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    public Tile(PointF center, string imagePath) {
        Position = new(center, SIZE - 2, SIZE - 2);
        Rect = Collision.GetBoundingBox(Position);
        ImagePath = imagePath;
    }

    // Static tile creation methods --------------------------------------------------------------------------------------------------------
    public static List<Tile> CreateTiles(List<(int x, int y)> positionsOfTiles, string texturePath)
    {
        var tiles = new List<Tile>();
        foreach (var (x, y) in positionsOfTiles)
        {
            tiles.Add(CreateTile(x, y, texturePath));
        }
        return tiles;
    }

    public static Tile CreateTile(int x, int y, string imagePath)
    {
        return new Tile(new PointF(x * SIZE + HALF_SIZE, y * SIZE + HALF_SIZE), imagePath);
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, (Map Map, bool Scale) @params) {
        var (map, scale) = @params;
        var point = scale ? map.ToScreen(Center) : map.ToCanvas(Center);
        int size = scale ? map.ToScreenWidth(SIZE) : SIZE;
        if (ImageReferrer.Get(ImagePath) is not ElementReference img) return false;
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
