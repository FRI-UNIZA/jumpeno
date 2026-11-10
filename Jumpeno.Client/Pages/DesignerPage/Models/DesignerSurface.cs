namespace Jumpeno.Client.Models;

public class DesignerSurface(Surface surface) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Surface Surface { get; private set; } = surface;
    private readonly List<DesignerSurface> Children = [];

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Add(DesignerSurface surface) => Children.Add(surface);

    public IEnumerable<DesignerSurface> Iterator { get {
        foreach (var surface in Children) {
            yield return surface;
        }
    }}
};
