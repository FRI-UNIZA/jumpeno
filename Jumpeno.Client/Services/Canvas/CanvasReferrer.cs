namespace Jumpeno.Client.Services;

public static class CanvasReferrer {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<CanvasType, Canvas2DContext> Refs = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static async Task Set(CanvasType id, BECanvasComponent? reference) {
        if (reference == null) return;
        var ctx = await reference.CreateCanvas2DAsync();
        if (ctx == null) return;
        Refs[id] = ctx;
    }

    public static Canvas2DContext? Get(CanvasType id) {
        Refs.TryGetValue(id, out var ctx);
        return ctx;
    }
}
