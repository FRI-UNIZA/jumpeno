namespace Jumpeno.Shared.Interfaces;

// Base -----------------------------------------------------------------------------------------------------------------------------------
public interface IRenderable {
    public Task<bool> Render(Canvas2DContext ctx, object? @params = null) => throw new NotImplementedException();
}

// Generic --------------------------------------------------------------------------------------------------------------------------------
public interface IRenderable<T> : IRenderable {
    public async new sealed Task<bool> Render(Canvas2DContext ctx, object? @params = null) {
        if (@params is not T parameters) return false;
        return await Render(ctx, parameters);
    }
    public Task<bool> Render(Canvas2DContext ctx, T @params);
}
