namespace Jumpeno.Client.Models;

public record UpdateAfterEvent(
    GameUpdate Update,
    bool Success
);
