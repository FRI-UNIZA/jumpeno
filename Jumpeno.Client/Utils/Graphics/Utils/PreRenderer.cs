namespace Jumpeno.Client.Utils;

public class PreRenderer<T>(
    CanvasType id,
    Func<Canvas2DContext, T, Task<bool>> preRender,
    Func<(Canvas2DContext Source, Canvas2DContext Destination), T, Task<bool>> applyRender
) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public CanvasType ID { get; private set; } = id;
    private Canvas2DContext? Source => CanvasReferrer.Get(ID);
    
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private Func<Canvas2DContext, T, Task<bool>> PreRenderAction { get; set; } = preRender;
    private Func<(Canvas2DContext Source, Canvas2DContext Destination), T, Task<bool>> ApplyRenderAction { get; set; } = applyRender;

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public bool IsPrerendered { get; private set; } = false;

    public async Task<bool> PreRender(T @params) {
        IsPrerendered = false;
        if (Source == null) return IsPrerendered;
        IsPrerendered = await PreRenderAction(Source, @params);
        return IsPrerendered;
    }

    public async Task<bool> Render(Canvas2DContext ctx, T @params) {
        if (!IsPrerendered) await PreRender(@params);
        if (!IsPrerendered || Source == null) return false;
        return await ApplyRenderAction((Source, ctx), @params);
    }
}
