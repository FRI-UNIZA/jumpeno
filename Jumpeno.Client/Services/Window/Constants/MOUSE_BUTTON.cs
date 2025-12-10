namespace Jumpeno.Client.Constants;

public enum MOUSE_BUTTON
{
    LEFT,
    MIDDLE,
    RIGHT,
    OTHER
}

public static class MOUSE_BUTTON_Extension
{
    public static long Raw(this MOUSE_BUTTON button)
    {
        return button switch
        {
            MOUSE_BUTTON.LEFT => 0,
            MOUSE_BUTTON.MIDDLE => 1,
            MOUSE_BUTTON.RIGHT => 2,
            MOUSE_BUTTON.OTHER => 3,
            _ => throw new ArgumentException("Invalid value")
        };
    }

    public static MOUSE_BUTTON From(long value)
    {
        return value switch
        {
            0 => MOUSE_BUTTON.LEFT,
            1 => MOUSE_BUTTON.MIDDLE,
            2 => MOUSE_BUTTON.RIGHT,
            _ => MOUSE_BUTTON.OTHER
        };
    }
}
