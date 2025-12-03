namespace Jumpeno.Client.Models;

public record PreRenderedCanvas(
    CANVAS ID,
    int Width,
    int Height
) {
    public BECanvasComponent? Ref { get; set; } = null;
}
