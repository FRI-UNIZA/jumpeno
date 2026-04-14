namespace Jumpeno.Client.Components;

public partial class CanvasPreRenderer {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "project-canvas-pre-renderer";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly List<PreRenderedCanvas> List = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    protected static void Add(CanvasType id, float width, float height) => List.Add(new(id, (int) width, (int) height));
    protected static void Add(CanvasType id, int width, int height) => List.Add(new(id, width, height));

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    static CanvasPreRenderer() => Init();

    protected override bool ShouldComponentRender() => false;

    protected override async void OnComponentAfterRender(bool firstRender) {
        if (!firstRender) return;
        foreach (var canvas in List) {
            await CanvasReferrer.Set(canvas.ID, canvas.Ref);
        }
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    private static void Init() {
        // Add objects here:
        Add(CanvasType.MapBackground, Map.WIDTH, Map.HEIGHT);
        Add(CanvasType.MapTiles, Map.WIDTH, Map.HEIGHT);
        Add(CanvasType.TilePatern, Map.WIDTH, Map.HEIGHT);
    }
}
