namespace Jumpeno.Shared.Interfaces;

public interface IRenderablePure : IRenderable {
    public Task Render(Canvas2DContext ctx);
}
