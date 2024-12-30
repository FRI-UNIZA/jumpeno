namespace Jumpeno.Shared.Models;

public class Tile : IRectFPositionable, IRenderable<Game> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int SIZE = 64;
    public const int HALF_SIZE = SIZE / 2;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    [JsonInclude]
    private PointF Center => Position.Center;
    public RectFPosition Position { get; private set; }
    public RectangleF Rect { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public Tile(PointF center) {
        Position = new(center, SIZE - 2, SIZE - 2);
        Rect = Collision.GetBoundingBox(Position);
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task Render(Canvas2DContext ctx, Game game) {
        var p = game.Map.ToScreen(Position.Center);
        int size = (int) Math.Ceiling((double) game.Map.ToScreenWidth(SIZE));
        if (ImageReferrer.GetRef(IMAGE.TILE) is not ElementReference img) return;
        await ctx.DrawImageAsync(
            img,
            0, 0,
            SIZE, SIZE,
            Math.Floor((double) p.X - size / 2) - 0.5, Math.Floor((double) p.Y - size / 2) - 0.5,
            size + 1, size + 1
        );
    }
}
