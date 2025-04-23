namespace Jumpeno.Shared.Models;

public record AuthDeleteDTO(
    string RefreshToken
) : AuthRefreshDTO(
    RefreshToken
);
