namespace Jumpeno.Client.Components;

public enum DividerOrientation
{
    Left,
    Center,
    Right
}

public static class DividerOrientationExtension
{
    public static AntDesign.DividerOrientation ToAntDesignEnum(this DividerOrientation orientation)
    {
        return orientation switch
        {
            DividerOrientation.Left => AntDesign.DividerOrientation.Left,
            DividerOrientation.Center => AntDesign.DividerOrientation.Center,
            DividerOrientation.Right => AntDesign.DividerOrientation.Right,
            _ => throw new ArgumentException("Invalid orientation value")
        };
    }
}
