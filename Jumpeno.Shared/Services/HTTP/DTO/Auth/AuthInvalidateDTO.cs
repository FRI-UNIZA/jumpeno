namespace Jumpeno.Shared.Models;

public record AuthInvalidateDTO(
    string RefreshToken
) : AuthRefreshDTO(
    RefreshToken
);
