namespace Jumpeno.Shared.Models;

using System.Diagnostics;

public class Animation : IRenderableParametric<(Game Game, SKIN Skin, bool Alive, bool Jumping, PointF Center, PointF Direction)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int WIDTH = 64; // px
    public const int HEIGHT = 76; // px
    public const int SHIFT_LEFT = -6; // px
    public const int SHIFT_BOTTOM = 7; // px
    public const int IDLE_INTERVAL = 180; // ms
    public const int RUN_INTERVAL = 130; // ms

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private PointF Direction;
    private bool Running;
    private readonly Stopwatch Watch = new();

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public Animation(PointF direction) {
        Direction = direction;
        if (Direction.X == 0) Direction.X = 1;
        if (Direction.Y == 0) Direction.Y = -1;
        Running = direction.X != 0;
        Watch.Start();
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private void UpdateDirection(PointF direction) {
        if (direction.X != 0) Direction.X = direction.X;
        if (direction.Y != 0) Direction.Y = direction.Y;
        Running = direction.X != 0;
    }

    public static ElementReference? GetImage(SKIN skin) {
        if (AppEnvironment.IsServer) return null;
        string id = IMAGE.SPRITE_MAGE_MAGIC;
        switch (skin) {
            case SKIN.MAGE_AIR: id = IMAGE.SPRITE_MAGE_AIR; break;
            case SKIN.MAGE_EARTH: id = IMAGE.SPRITE_MAGE_EARTH; break;
            case SKIN.MAGE_FIRE: id = IMAGE.SPRITE_MAGE_FIRE; break;
            case SKIN.MAGE_MAGIC: id = IMAGE.SPRITE_MAGE_MAGIC; break;
            case SKIN.MAGE_WATER: id = IMAGE.SPRITE_MAGE_WATER; break;
        }
        if (ImageReferrer.GetRef(id) is not ElementReference img) return null;
        return img;
    }

    public (int X, int Y) GetSpritePosition(bool alive, bool jumping, PointF direction) {
        if (!alive) return (0, 2);
        if (jumping && direction.X == 0) return (1, 2);
        var divisor = Running ? RUN_INTERVAL : IDLE_INTERVAL;
        return (
            (int) (Watch.ElapsedMilliseconds / divisor % 4),
            Running ? 1 : 0
        );
    } 

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task Render(Canvas2DContext ctx, (Game Game, SKIN Skin, bool Alive, bool Jumping, PointF Center, PointF Direction) @params) {
        var (game, skin, alive, jumping, center, direction) = @params;
        // 1) Update direction & check image:
        UpdateDirection(direction);
        if (GetImage(skin) is not ElementReference img) return;
        // 2) Compute placement:
        var width = game.Map.ToScreenWidth(WIDTH);
        var height = game.Map.ToScreenHeight(HEIGHT);
        center = game.Map.ToScreen(new(center.X + SHIFT_LEFT * Direction.X, center.Y + SHIFT_BOTTOM));
        // 3) Pick sprite position:
        var pos = GetSpritePosition(alive, jumping, direction);
        // 4) Render sprite:
        await ctx.SaveAsync();
        await ctx.ScaleAsync(Direction.X, 1);
        await ctx.DrawImageAsync(
            img,
            pos.X * WIDTH, pos.Y * HEIGHT,
            WIDTH, HEIGHT,
            Math.Floor((double) Direction.X * center.X - width / 2), Math.Floor((double) center.Y - height / 2),
            width, height
        );
        await ctx.RestoreAsync();
    }
}
