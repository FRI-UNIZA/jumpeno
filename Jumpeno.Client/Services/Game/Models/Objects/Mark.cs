namespace Jumpeno.Client.Utils;

public static class Mark {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int MarkHeight = 10; // px

    public const int TextSize = 14; // px
    public const int MinTextSize = 6; // px
    public const int TextWeight = 500;

    public const int MarkBottomOffset = 10; // px
    public const int MarkTopOffset = 10; // px

    public static int Height => MarkBottomOffset + Math.Max(MarkHeight, TextSize) + MarkTopOffset;

    // Calculations -----------------------------------------------------------------------------------------------------------------------
    public static PointF CalculateMarkPoint(Body body) {
        var center = body.Position.Center;
        var animation = body.Animation;
        return new(
            center.X + Animation.ShiftLeft * animation.Direction.X,
            center.Y + Animation.ShiftBottom + Animation.HalfHeight + MarkBottomOffset
        );
    }

    public static PointF CalculateMarkPointTop(Body body) {
        var point = CalculateMarkPoint(body);
        return new(point.X, point.Y + Math.Max(MarkHeight, TextSize) + MarkTopOffset);
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> RenderMark(Canvas2DContext ctx, (Game Game, Player Player) @params) {
        // 1) Parameters:
        var (game, player) = @params;
        // 2) Comput mark points:
        var point = CalculateMarkPoint(player.Body);
        var pointLeft = game.Map.ToScreen(new(point.X - MarkHeight, point.Y + MarkHeight));
        var pointRight = game.Map.ToScreen(new(point.X + MarkHeight, point.Y + MarkHeight));
        point = game.Map.ToScreen(point);
        // 3) Render mark:
        await ctx.SetFillStyleAsync($"{game.Map.Foreground}");
        await ctx.BeginPathAsync();
        await ctx.MoveToAsync(point.X, point.Y);
        await ctx.LineToAsync(pointLeft.X, pointLeft.Y);
        await ctx.LineToAsync(pointRight.X, pointRight.Y);
        await ctx.FillAsync();
        return true;
    }

    public static async Task<bool> RenderName(Canvas2DContext ctx, (Game Game, Player Player, string Font) @params) {
        // 1) Parameters:
        var (game, player, font) = @params;
        var size = Math.Max(game.Map.ToScreenHeight(TextSize), MinTextSize * game.Map.Dpr);
        // 2) Compute mark point:
        var point = game.Map.ToScreen(CalculateMarkPoint(player.Body));
        // 3) Render name:
        await ctx.SetFontAsync($"{TextWeight} {size}px {font}");
        await ctx.SetFillStyleAsync($"{game.Map.Foreground}");
        await ctx.SetTextBaselineAsync(TextBaseline.Alphabetic);
        await ctx.SetTextAlignAsync(TextAlign.Center);
        await ctx.FillTextAsync(player.User.Name, point.X, point.Y);
        return true;
    }
}
