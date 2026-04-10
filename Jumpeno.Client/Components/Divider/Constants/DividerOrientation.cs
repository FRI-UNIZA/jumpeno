namespace Jumpeno.Client.Components;

public enum DividerOrientation
{
    LEFT,
    CENTER,
    RIGHT
}

public static class DividerOrientationExtension
{
    public static AntDesign.DividerOrientation ToAntDesignEnum(this DividerOrientation orientation)
    {
        return orientation switch
        {
            DividerOrientation.LEFT => AntDesign.DividerOrientation.Left,
            DividerOrientation.CENTER => AntDesign.DividerOrientation.Center,
            DividerOrientation.RIGHT => AntDesign.DividerOrientation.Right,
            _ => throw new ArgumentException("Invalid orientation value")
        };
    }
}
