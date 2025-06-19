namespace Jumpeno.Client.Models;

public record struct CreateData(
    string? Code,
    string GameName,
    string? Map,
    GAME_MODE GameMode,
    DISPLAY_MODE DisplayMode,
    bool Presentation,
    byte Capacity,
    bool Anonyms
);
