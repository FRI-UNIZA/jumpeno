namespace Jumpeno.Shared.Models;

public class Map {
    // World ------------------------------------------------------------------------------------------------------------------------------
    public float WorldMinX { get; private set; }
    public float WorldMaxX { get; private set; }
    public float WorldMinY { get; private set; }
    public float WorldMaxY { get; private set; }
    public float WorldWidth => Math.Abs(WorldMaxX - WorldMinX);
    public float WorldHeight => Math.Abs(WorldMaxY - WorldMinY);

    // Screen -----------------------------------------------------------------------------------------------------------------------------
    public int ScreenMinX { get; private set; } = 0;
    public int ScreenMaxX { get; private set; } = 0;
    public int ScreenMinY { get; private set; } = 0;
    public int ScreenMaxY { get; private set; } = 0;
    public int ScreenWidth => Math.Abs(ScreenMaxX - ScreenMinX);
    public int ScreenHeight => Math.Abs(ScreenMaxY - ScreenMinY);

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private Map(
        float worldMinX, float worldMaxX, float worldMinY, float worldMaxY,
        int screenMinX, int screenMaxX, int screenMinY, int screenMaxY
    ) {
        WorldMinX = worldMinX;
        WorldMaxX = worldMaxX;
        WorldMinY = worldMinY;
        WorldMaxY = worldMaxY;
        ScreenMinX = screenMinX;
        ScreenMaxX = screenMaxX;
        ScreenMinY = screenMinY;
        ScreenMaxY = screenMaxY;
    }

    public Map(float minX, float maxX, float minY, float maxY): this(minX, maxX, minY, maxY, 0, 0, 0, 0) {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void UpdateScreen(int minX, int maxX, int minY, int maxY) {
        ScreenMinX = minX;
        ScreenMaxX = maxX;
        ScreenMinY = minY;
        ScreenMaxY = maxY;
    }

    public Point ToScreen(PointF point) {
        return new Point(
            (int) ((point.X - WorldMinX) / (WorldMaxX - WorldMinX) * (ScreenMaxX - ScreenMinX) + ScreenMinX),
            (int) ((point.Y - WorldMinY) / (WorldMaxY - WorldMinY) * (ScreenMaxY - ScreenMinY) + ScreenMinY)
        );
    }

    public PointF ToWorld(Point point) {
        return new PointF(
            (point.X - ScreenMinX) / (float) (ScreenMaxX - ScreenMinX) * (WorldMaxX - WorldMinX) + WorldMinX,
            (point.Y - ScreenMinY) / (float) (ScreenMaxY - ScreenMinY) * (WorldMaxY - WorldMinY) + WorldMinY
        );
    }
}
