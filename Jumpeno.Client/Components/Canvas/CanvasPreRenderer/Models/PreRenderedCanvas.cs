namespace Jumpeno.Client.Models;

public record PreRenderedCanvas(
    CanvasType ID,
    int Width,
    int Height
) {
    public BECanvasComponent? Ref { get; set; } = null;
}
