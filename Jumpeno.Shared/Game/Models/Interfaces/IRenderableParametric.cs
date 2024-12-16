namespace Jumpeno.Shared.Interfaces;

public interface IRenderableParametric<T> : IRenderable {
    public Task Render(Canvas2DContext ctx, T parameters);
}
