namespace Jumpeno.Client.Models;

public record RegisteredGameParams(
    string Code,
    DEVICE_TYPE Device,
    string AccessToken
);
