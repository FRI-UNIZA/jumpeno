namespace Jumpeno.Client.Components;

public partial class CanvasPreRenderer {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "project-canvas-pre-renderer";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly List<PreRenderedCanvas> List = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static void Add(CANVAS id, float width, float height) => List.Add(new(id, (int) width, (int) height));
    private static void Add(CANVAS id, int width, int height) => List.Add(new(id, width, height));

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    static CanvasPreRenderer() {
        Add(CANVAS.TILE_PATTERN, Map.WIDTH, Map.HEIGHT);
        Add(CANVAS.MAP, Map.WIDTH, Map.HEIGHT);
    }

    protected override async void OnAfterRender(bool firstRender) {
        if (!firstRender) return;
        foreach (var canvas in List) {
            await CanvasReferrer.Set(canvas.ID, canvas.Ref);
        }
    }
}
