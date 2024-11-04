using Blazor.Extensions.Canvas.Canvas2D;

namespace Jumpeno.Shared.Models;

public class Avatar {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int WIDTH = 64;
    public const int HEIGHT = 76;
    public const double SPEED = 0.2; // px per ms

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Alive { get; set; }
    public PointF Position { get; set; }
    public PointF Direction { get; set; }
    public int Kills { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Avatar(bool alive, PointF position, PointF direction, int kills) {
        Alive = alive;
        Position = position;
        Direction = direction;
        Kills = kills;
    }
    public Avatar(): this(false, new(500, 0), new(-1, 0), 0) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void Update(double deltaT, Game game) {
        var newX = (float) (Position.X + deltaT * Direction.X * SPEED);
        if (newX + WIDTH >= game.Map.WorldMaxX) newX = game.Map.WorldMaxX - WIDTH;
        else if (newX <= game.Map.WorldMinX) newX = game.Map.WorldMinX;

        Position = new PointF(newX, Position.Y);
    }

    public async Task Draw(Canvas2DContext ctx, Map map, SKIN? skin = null) {
        if (skin == null) skin = SKIN.MAGE_MAGIC;
        var p1 = map.ToScreen(Position);
        var p2 = map.ToScreen(new PointF(Position.X + WIDTH, Position.Y + HEIGHT));
        await ctx.SetFillStyleAsync("red");
        await ctx.FillRectAsync(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
    }
}
