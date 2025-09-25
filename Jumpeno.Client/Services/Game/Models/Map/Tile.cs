namespace Jumpeno.Client.Models;

public class Tile : IRectFPositionable, IRenderable<(Game Game, bool Scale)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int SIZE = 64;
    public const int HALF_SIZE = SIZE / 2;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    [JsonInclude]
    private PointF Center => Position.Center;
    public RectFPosition Position { get; private set; }
    public RectangleF Rect { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public Tile(PointF center) {
        Position = new(center, SIZE - 2, SIZE - 2);
        Rect = Collision.GetBoundingBox(Position);
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, (Game Game, bool Scale) @params) {
        var (game, scale) = @params;
        var point = scale ? game.Map.ToScreen(Center) : game.Map.ToCanvas(Center);
        int size = scale ? game.Map.ToScreenWidth(SIZE) : SIZE;
        if (ImageReferrer.Get(IMAGE.TILE) is not ElementReference img) return false;
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
