namespace Jumpeno.Client.Models;

using System.Diagnostics;

public class Animation : IRenderable<(Game Game, Skin Skin, Body Body)> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int Width = 64; // px
    public const int Height = 76; // px
    public const int HalfWidth = Width / 2;
    public const int HalfHeight = Height / 2;
    public const int MaxWidth = 100; // px
    public const int MaxHeight = 100; // px

    public const int ShiftLeft = -6; // px
    public const int ShiftBottom = 7; // px

    public const int IdleInterval = 200; // ms
    public const int RunInterval = 130; // ms

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public PointF Direction { get { return _direction; } private set { _direction = value; } } private PointF _direction;
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
        if (Direction.X == 0) _direction.X = rand.NextDouble() < 0.5 ? 1 : -1;
        if (Direction.Y == 0) _direction.Y = -1;
        Running = direction.X != 0;
        Watch.Start();
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public void ResetDirection(PointF direction) {
        _direction.X = direction.X;
        _direction.Y = direction.Y;
        Running = false;
    }

    public void UpdateDirection(PointF direction) {
        if (direction.X != 0) _direction.X = direction.X;
        if (direction.Y != 0) _direction.Y = direction.Y;
        Running = direction.X != 0;
    }

    private static ElementReference? GetImage(Skin skin) {
        if (AppEnvironment.IsServer) return null;
        string id = skin.ToImagePath();
        if (ImageReferrer.Get(id) is not ElementReference img) return null;
        return img;
    }

    private (int X, int Y) GetSpritePosition(bool alive, bool jumping) {
        if (!alive) return (0, 2);
        if (jumping) return (1, 2);
        var divisor = Running ? RunInterval : IdleInterval;
        return (
            (int) (Watch.ElapsedMilliseconds / divisor % 4),
            Running ? 1 : 0
        );
    } 

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, (Game Game, Skin Skin, Body Body) @params) {
        var (game, skin, body) = @params;
        // 1) Check image:
        if (GetImage(skin) is not ElementReference img) return false;
        // 2) Compute placement:
        var width = game.Map.ToScreenWidth(Width);
        var height = game.Map.ToScreenHeight(Height);
        var center = game.Map.ToScreen(
            new(body.Position.Center.X + ShiftLeft * Direction.X, body.Position.Center.Y + ShiftBottom)
        );
        // 3) Pick sprite position:
        var pos = GetSpritePosition(body.Alive, body.IsJumping);
        // 4) Render sprite:
        await ctx.SaveAsync();
        await ctx.ScaleAsync(Direction.X, 1);
        if (body.IsImmortal) await ctx.SetGlobalAlphaAsync(0.5f);
        await ctx.DrawImageAsync(
            img,
            pos.X * (Width + 1), pos.Y * (Height + 1),
            Width, Height,
            Math.Floor((double) Direction.X * center.X - width / 2), Math.Floor((double) center.Y - height / 2),
            width, height
        );
        await ctx.RestoreAsync();
        return true;
    }
}
