namespace Jumpeno.Client.Constants;

public class COMMON_IMAGE {
    public readonly Func<string> BG_YELLOW = () => URL.ImageLink("backgrounds/bg-yellow.svg");
    public readonly Func<string> BG_BLUE = () => URL.ImageLink("backgrounds/bg-blue.svg");
    public readonly Func<string> JUMPER_YELLOW = () => URL.ImageLink("images/jumper-yellow.png");
    public readonly Func<string> JUMPENO_PLAYERS = () => URL.ImageLink("images/jumpeno-players.png");
    public readonly Func<string> KEYBOARD = () => URL.ImageLink("images/keyboard.png");
}
public class LANGUAGE_IMAGE {}

public static class IMAGE {
    public static readonly COMMON_IMAGE COMMON = new();
    public static readonly LANGUAGE_IMAGE LANGUAGE = new();
}
