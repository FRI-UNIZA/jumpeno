namespace Jumpeno.Client.Enums;

public enum MouseButton
{
    Left,
    Middle,
    Right,
    Other
}

public static class MouseButtonExtension
{
    public static long Raw(this MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => 0,
            MouseButton.Middle => 1,
            MouseButton.Right => 2,
            MouseButton.Other => 3,
            _ => throw new ArgumentException("Invalid value")
        };
    }

    public static MouseButton From(long value)
    {
        return value switch
        {
            0 => MouseButton.Left,
            1 => MouseButton.Middle,
            2 => MouseButton.Right,
            _ => MouseButton.Other
        };
    }
}
