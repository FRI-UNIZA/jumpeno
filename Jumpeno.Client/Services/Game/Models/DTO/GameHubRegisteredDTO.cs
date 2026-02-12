namespace Jumpeno.Client.Models;

public record GameHubRegisteredDTO(
    // Game:
    string Code,
    // Player:
    string AccessToken,
    DEVICE_TYPE Device,
    bool Spectate
) : IValidable<GameHubRegisteredDTO> {
    public List<Error> Validate() {
        var errors = GameValidator.ValidateCode(Code, nameof(Code));
        errors.AddRange(TokenValidator.ValidateToken(AccessToken, nameof(AccessToken)));
        errors.AddRange(UserValidator.ValidateDeviceType(Device, nameof(Device)));
        errors.AddRange(Checker.ValidateUndefined(Spectate, nameof(Spectate)));
        return errors;
    }
    public GameHubRegisteredDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}
