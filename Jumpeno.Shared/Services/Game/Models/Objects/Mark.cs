namespace Jumpeno.Shared.Utils;

public static class Mark {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int MARK_HEIGHT = 10; // px

    public const int TEXT_SIZE = 14; // px
    public const int MIN_TEXT_SIZE = 6; // px

    public const int MARK_BOTTOM_OFFSET = 10; // px
    public const int MARK_TOP_OFFSET = 10; // px

    public static int HEIGHT => MARK_BOTTOM_OFFSET + Math.Max(MARK_HEIGHT, TEXT_SIZE) + MARK_TOP_OFFSET;

    // Calculations -----------------------------------------------------------------------------------------------------------------------
    public static PointF CalculateMarkPoint(Body body) {
        var center = body.Position.Center;
        var animation = body.Animation;
        return new(
            center.X + Animation.SHIFT_LEFT * animation.Direction.X,
            center.Y + Animation.SHIFT_BOTTOM + Animation.HALF_HEIGHT + MARK_BOTTOM_OFFSET
        );
    }

    public static PointF CalculateMarkPointTop(Body body) {
        var point = CalculateMarkPoint(body);
        return new(point.X, point.Y + Math.Max(MARK_HEIGHT, TEXT_SIZE) + MARK_TOP_OFFSET);
    }

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> RenderMark(Canvas2DContext ctx, (Game Game, Player Player) @params) {
        // 1) Parameters:
        var (game, player) = @params;
        // 2) Comput mark points:
        var point = CalculateMarkPoint(player.Body);
        var pointLeft = game.Map.ToScreen(new(point.X - MARK_HEIGHT, point.Y + MARK_HEIGHT));
        var pointRight = game.Map.ToScreen(new(point.X + MARK_HEIGHT, point.Y + MARK_HEIGHT));
        point = game.Map.ToScreen(point);
        // 3) Render mark:
        await ctx.SetFillStyleAsync($"rgb({game.Map.Foreground})");
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
        var size = Math.Max(game.Map.ToScreenHeight(TEXT_SIZE), MIN_TEXT_SIZE);
        // 2) Compute mark point:
        var point = game.Map.ToScreen(CalculateMarkPoint(player.Body));
        // 3) Render name:
        await ctx.SetFontAsync($"{size}px {font}");
        await ctx.SetFillStyleAsync($"rgb({game.Map.Foreground})");
        await ctx.SetTextBaselineAsync(TextBaseline.Alphabetic);
        await ctx.SetTextAlignAsync(TextAlign.Center);
        await ctx.FillTextAsync(player.User.Name, point.X, point.Y);
        return true;
    }
}
