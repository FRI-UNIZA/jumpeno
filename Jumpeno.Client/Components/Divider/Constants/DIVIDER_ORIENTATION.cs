namespace Jumpeno.Client.Components;

public enum DIVIDER_ORIENTATION
{
    LEFT,
    CENTER,
    RIGHT
}

public static class DIVIDER_ORIENTATION_Extension
{
    public static AntDesign.DividerOrientation ToAntDesignEnum(this DIVIDER_ORIENTATION orientation)
    {
        return orientation switch
        {
            DIVIDER_ORIENTATION.LEFT => AntDesign.DividerOrientation.Left,
            DIVIDER_ORIENTATION.CENTER => AntDesign.DividerOrientation.Center,
            DIVIDER_ORIENTATION.RIGHT => AntDesign.DividerOrientation.Right,
            _ => throw new ArgumentException("Invalid orientation value")
        };
    }
}
