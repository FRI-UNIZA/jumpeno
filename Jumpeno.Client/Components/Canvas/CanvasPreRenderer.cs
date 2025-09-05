namespace Jumpeno.Client.Components;

public partial class CanvasPreRenderer {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "project-canvas-pre-renderer";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly List<PreRenderedCanvas> List = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    protected static void Add(CANVAS id, float width, float height) => List.Add(new(id, (int) width, (int) height));
    protected static void Add(CANVAS id, int width, int height) => List.Add(new(id, width, height));

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    static CanvasPreRenderer() => Init();

    protected override async void OnComponentAfterRender(bool firstRender) {
        if (!firstRender) return;
        foreach (var canvas in List) {
            await CanvasReferrer.Set(canvas.ID, canvas.Ref);
        }
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    private static void Init() {
        // Add objects here:
        Add(CANVAS.TILE_PATTERN, Map.WIDTH, Map.HEIGHT);
        Add(CANVAS.MAP, Map.WIDTH, Map.HEIGHT);
    }
}
