namespace Jumpeno.Shared.Models;

public record AuthRefreshDTOR(
    string AccessToken,
    string RefreshToken
) : UserLoginDTOR(
    AccessToken,
    RefreshToken
);
