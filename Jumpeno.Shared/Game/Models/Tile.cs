namespace Jumpeno.Shared.Models;

public class Tile : IRectFPositionable, IRenderableParametric<Game> {
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
        Position = new(center, SIZE, SIZE);
        Rect = Collision.GetBoundingBox(Position);
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task Render(Canvas2DContext ctx, Game game) {
        var p = game.Map.ToScreen(Position.Center);
        int size = (int) Math.Ceiling((double) game.Map.ToScreenWidth(SIZE));
        var imgRef = ImageReferrer.GetRef(IMAGE.COMMON.SPRITE_TILE);
        if (imgRef is ElementReference img) {
            await ctx.DrawImageAsync(
                img,
                0, 0,
                SIZE, SIZE,
                Math.Floor((double) p.X - size / 2) - 1, Math.Floor((double) p.Y - size / 2) - 1,
                size + 2, size + 2 
            );
        }
    }
}
