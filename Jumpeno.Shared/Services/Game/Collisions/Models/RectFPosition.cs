namespace Jumpeno.Shared.Models;

public record struct RectFPosition {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public PointF Center;
    public float Width;
    public float Height;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public RectFPosition(PointF center, float width, float height) {
        Center = center;
        Width = width;
        Height = height;
    }
}
