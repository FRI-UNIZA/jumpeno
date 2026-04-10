namespace Jumpeno.Client.Constants;

public enum MouseButton
{
    LEFT,
    MIDDLE,
    RIGHT,
    OTHER
}

public static class MouseButtonExtension
{
    public static long Raw(this MouseButton button)
    {
        return button switch
        {
            MouseButton.LEFT => 0,
            MouseButton.MIDDLE => 1,
            MouseButton.RIGHT => 2,
            MouseButton.OTHER => 3,
            _ => throw new ArgumentException("Invalid value")
        };
    }

    public static MouseButton From(long value)
    {
        return value switch
        {
            0 => MouseButton.LEFT,
            1 => MouseButton.MIDDLE,
            2 => MouseButton.RIGHT,
            _ => MouseButton.OTHER
        };
    }
}
