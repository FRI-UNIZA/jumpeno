namespace Jumpeno.Shared.Services;

public static class CanvasReferrer {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<CANVAS, Canvas2DContext> Refs = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static async Task Set(CANVAS id, BECanvasComponent? reference) {
        if (reference == null) return;
        var ctx = await reference.CreateCanvas2DAsync();
        if (ctx == null) return;
        Refs[id] = ctx;
    }

    public static Canvas2DContext? Get(CANVAS id) {
        if (!Refs.TryGetValue(id, out var ctx)) return null;
        return ctx;
    }
}
