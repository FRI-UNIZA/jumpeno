namespace Jumpeno.Client.Models;

public record CreateGameParams(
    string? Code,
    DEVICE_TYPE Device,
    string AccessToken,
    string GameName,
    string? Map,
    GAME_MODE GameMode,
    DISPLAY_MODE DisplayMode,
    bool Presentation,
    byte Capacity,
    bool Anonyms
);
