namespace Jumpeno.Client.Models;

public class Shrink : IUpdateable, IPreRendered<Game> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Level:
    public static (int LEVEL, double TIMER) Default => (-1, Duration);
    public const int MaxLevel = ((int) Map.Width + Tile.Size) / Tile.Size / 2;
    // Alpha:
    public const double MinAlpha = 0.2;
    public const double MaxAlpha = 0.5;
    public const double LumaThreshold = 220;
    // Duration:
    public const double FadeInMs = 400; // ms
    public const double MarkMs = 3000; // ms
    public const double HighlightMs = 2000; // ms
    public const double Duration = MarkMs + HighlightMs; // ms
    public const double TotalDuration = Duration * MaxLevel; // ms
    // Blick:
    public const double BlickIntervalMs = 800; // ms
    public const double HalfBlickIntervalMs = BlickIntervalMs / 2; // ms

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Level:
    public int Level { get; private set; }
    public double Timer { get; private set; }
    // Alpha:
    public float Alpha { get {
        var t = Math.Max(Timer - MarkMs, 0) % BlickIntervalMs;
        var alpha = Level == 0 && Timer < FadeInMs ? MinAlpha * (Timer % FadeInMs / FadeInMs) : MinAlpha;
        alpha += (MaxAlpha - MinAlpha) * (1 - Math.Abs(HalfBlickIntervalMs - t) / HalfBlickIntervalMs);
        return (float)alpha;
    } }
    // Color:
    public static RGBAColor Color(RGBColor tint, float alpha) => new(tint, alpha);

    [JsonInclude][Newtonsoft.Json.JsonProperty] private float WorldX { get; set; }
    [JsonInclude][Newtonsoft.Json.JsonProperty] private float WorldY { get; set; }
    [JsonInclude][Newtonsoft.Json.JsonProperty] private float WorldWidth { get; set; }
    [JsonInclude][Newtonsoft.Json.JsonProperty] private float WorldHeight { get; set; }
    public RectangleF Rect => new(
        WorldX + Math.Max(Level, 0) * Tile.Size, WorldY,
        WorldWidth - 2 * Math.Max(Level, 0) * Tile.Size, WorldHeight
    );

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    public Shrink(int level, double timer, float worldX, float worldY, float worldWidth, float worldHeight) {
        // Properties:
        Level = level;
        Timer = timer;
        // World dimensions:
        WorldX = worldX;
        WorldY = worldY;
        WorldWidth = worldWidth;
        WorldHeight = worldHeight;
        // Prerender:
        PreRenderer = InitPrerenderer();
    }

    public Shrink(Map map) : this(Default.LEVEL, Default.TIMER, 0, 0, 0, 0) {
        var rect = map.Rect;
        WorldX = rect.X;
        WorldY = rect.Y;
        WorldWidth = rect.Width;
        WorldHeight = rect.Height;
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public bool Update(GameUpdate update)
    => update switch {
        TimeFlowUpdate time => TimeFlowUpdate(time),
        StateUpdate state => StateUpdate(state),
        _ => false
    };

    private bool TimeFlowUpdate(TimeFlowUpdate update) {
        if (update.Game.State != GameStates.Shrinking) return false;
        Timer += update.DeltaT;
        return true;
    }

    private bool StateUpdate(StateUpdate update) {
        Level = update.Level;
        Timer = update.Timer;
        return true;
    }

    // Pre-Rendering ----------------------------------------------------------------------------------------------------------------------
    private readonly PreRenderer<Game> PreRenderer;
    private PreRenderer<Game> InitPrerenderer() => new(CanvasType.TilePatern, PreRender, ApplyRender);
    private async Task<bool> PreRender(Canvas2DContext ctx, Game game) {
        // 1) Initialize:
        var prerendered = false;
        // 2) Clear:
        await ctx.ClearRectAsync(WorldX, WorldY, WorldWidth, WorldHeight);
        // 3) Render pattern:
        for (int i = 0, x = (int) WorldX; x < WorldX + WorldWidth; x += Tile.Size) {
            for (float y = WorldY; y < WorldY + WorldHeight; y += Tile.Size, i++) {
                var tile = new Tile(new(x + Tile.HalfSize, y + Tile.HalfSize));
                if (!await tile.Render(ctx, (game.Map, false))) break;
                if (i > 0) continue;
                prerendered = true;
            }
        }
        // 4) Return result:
        return prerendered;
    }
    private async Task<bool> ApplyRender((Canvas2DContext Source, Canvas2DContext Destination) context, Game game) {
        // 1) Check state & init:
        if (game.State != GameStates.Shrinking || Level < 0) return false;
        var (source, ctx) = context; var rect = Rect;

        // 2) Highlight area color & size:
        await ctx.SetFillStyleAsync($"{Color(game.Map.Tint, Alpha)}");
        var add = rect.Width < 2 * Tile.Size + Tile.HalfSize ? 4 : 1; var screen = game.Map.ScreenRect;
        var size = new Size(game.Map.ToScreenWidth(Tile.Size + Tile.HalfSize) + add, game.Map.ToScreenHeight(rect.Height) + 1);
        if (Level < MaxLevel - 1) {
            // 2.1) Left part:
            var point = game.Map.ToScreen(new(rect.X - Tile.HalfSize, rect.Y + rect.Height));
            await ctx.FillRectAsync(point.X - 0.5, point.Y - 0.5, size.Width, size.Height);
            // 2.2) Right part:
            point = game.Map.ToScreen(new(rect.X + rect.Width - Tile.Size, rect.Y + rect.Height));
            await ctx.FillRectAsync(point.X - 0.5, point.Y - 0.5, size.Width, size.Height);
        } else {
            // 2.3) Middle part:
            await ctx.FillRectAsync(screen.X, screen.Y, screen.Width, screen.Height);
        }

        // 3.1) Tiles size:
        if (Level <= 0) return true;
        var sizeF = new SizeF(rect.X - WorldX, rect.Height);
        size = new(game.Map.ToScreenWidth(sizeF.Width), game.Map.ToScreenHeight(sizeF.Height));
        // 3.2) Left part:
        await ctx.DrawImageAsync(
            source.Canvas, WorldX, WorldY, sizeF.Width, sizeF.Height,
            screen.X - 0.5, screen.Y - 0.5, size.Width + 2.5, screen.Height + 1
        );
        // 3.3) Right part:
        await ctx.DrawImageAsync(
            source.Canvas, WorldX, WorldY, sizeF.Width, sizeF.Height,
            screen.X + screen.Width - size.Width - 2, screen.Y - 0.5, size.Width + 2.5, screen.Height + 1
        ); return true;
    }
    public bool IsPrerendered => PreRenderer.IsPrerendered;
    public async Task<bool> PreRender(Game game) => await PreRenderer.PreRender(game);

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Render(Canvas2DContext ctx, Game game) {
        return await PreRenderer.Render(ctx, game);
    }
}
