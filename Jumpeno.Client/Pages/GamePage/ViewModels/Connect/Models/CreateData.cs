namespace Jumpeno.Client.Models;

public record struct CreateData(
    string? Code,
    string GameName,
    int? Map,
    bool Anonyms,
    byte Rounds,
    byte Capacity,
    DisplayMode DisplayMode,
    GameMode GameMode
);
