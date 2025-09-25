namespace Jumpeno.Client.Models;

using System.Diagnostics;

public class Animation : IRenderable<(Game Game, SKIN Skin, Body Body)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int WIDTH = 64; // px
    public const int HEIGHT = 76; // px
    public const int HALF_WIDTH = WIDTH / 2;
    public const int HALF_HEIGHT = HEIGHT / 2;
    public const int MAX_WIDTH = 100; // px
    public const int MAX_HEIGHT = 100; // px

    public const int SHIFT_LEFT = -6; // px
    public const int SHIFT_BOTTOM = 7; // px

    public const int IDLE_INTERVAL = 200; // ms
    public const int RUN_INTERVAL = 130; // ms

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public PointF Direction { get { return _Direction; } private set { _Direction = value; } } private PointF _Direction;
    public bool Running { get; private set; }
    private readonly Stopwatch Watch = new();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    protected Animation(PointF direction, bool running) {
        Direction = direction;
        Running = running;
        Watch.Start();
    }

    public Animation(PointF direction) {
        var rand = new Random();
        Direction = direction;
        if (Direction.X == 0) _Direction.X = rand.NextDouble() < 0.5 ? 1 : -1;
        if (Direction.Y == 0) _Direction.Y = -1;
        Running = direction.X != 0;
        Watch.Start();
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public void UpdateDirection(PointF direction) {
        if (direction.X != 0) _Direction.X = direction.X;
        if (direction.Y != 0) _Direction.Y = direction.Y;
        Running = direction.X != 0;
    }

    public static string GetURL(SKIN skin) {
        switch (skin) {
            case SKIN.MAGE_AIR: return IMAGE.SPRITE_MAGE_AIR;
            case SKIN.MAGE_EARTH: return IMAGE.SPRITE_MAGE_EARTH;
            case SKIN.MAGE_FIRE: return IMAGE.SPRITE_MAGE_FIRE;
            case SKIN.MAGE_MAGIC: return IMAGE.SPRITE_MAGE_MAGIC;
            case SKIN.MAGE_WATER: return IMAGE.SPRITE_MAGE_WATER;
            default: return IMAGE.SPRITE_MAGE_MAGIC;
        }
    }

    private static ElementReference? GetImage(SKIN skin) {
        if (AppEnvironment.IsServer) return null;
        string id = GetURL(skin);
        if (ImageReferrer.Get(id) is not ElementReference img) return null;
        return img;
    }

    private (int X, int Y) GetSpritePosition(bool alive, bool jumping) {
        if (!alive) return (0, 2);
        if (jumping) return (1, 2);
        var divisor = Running ? RUN_INTERVAL : IDLE_INTERVAL;
        return (
            (int) (Watch.ElapsedMilliseconds / divisor % 4),
            Running ? 1 : 0
        );
    } 

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, (Game Game, SKIN Skin, Body Body) @params) {
        var (game, skin, body) = @params;
        // 1) Update direction & check image:
        UpdateDirection(body.Direction);
        if (GetImage(skin) is not ElementReference img) return false;
        // 2) Compute placement:
        var width = game.Map.ToScreenWidth(WIDTH);
        var height = game.Map.ToScreenHeight(HEIGHT);
        var center = game.Map.ToScreen(
            new(body.Position.Center.X + SHIFT_LEFT * Direction.X, body.Position.Center.Y + SHIFT_BOTTOM)
        );
        // 3) Pick sprite position:
        var pos = GetSpritePosition(body.Alive, body.IsJumping);
        // 4) Render sprite:
        await ctx.SaveAsync();
        await ctx.ScaleAsync(Direction.X, 1);
        if (body.IsImmortal) await ctx.SetGlobalAlphaAsync(0.5f);
        await ctx.DrawImageAsync(
            img,
            pos.X * (WIDTH + 1), pos.Y * (HEIGHT + 1),
            WIDTH, HEIGHT,
            Math.Floor((double) Direction.X * center.X - width / 2), Math.Floor((double) center.Y - height / 2),
            width, height
        );
        await ctx.RestoreAsync();
        return true;
    }
}
