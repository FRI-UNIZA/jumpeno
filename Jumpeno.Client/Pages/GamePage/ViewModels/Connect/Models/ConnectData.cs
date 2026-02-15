namespace Jumpeno.Client.Models;

public record struct ConnectData(
    string Code,
    string Name,
    bool Spectate
);
