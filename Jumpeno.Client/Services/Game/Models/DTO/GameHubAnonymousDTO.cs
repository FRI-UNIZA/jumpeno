namespace Jumpeno.Client.Models;

public record GameHubAnonymousDTO(
    // Game:
    string Code,
    // Player:
    string Name,
    DEVICE_TYPE Device,
    bool Spectate
) : IValidable<GameHubAnonymousDTO> {
    public List<Error> Validate() {
        var errors = GameValidator.ValidateCode(Code, nameof(Code));
        errors.AddRange(UserValidator.ValidateName(Name, true, nameof(Name)));
        errors.AddRange(UserValidator.ValidateDeviceType(Device, nameof(Device)));
        errors.AddRange(Checker.ValidateUndefined(Spectate, nameof(Spectate)));
        return errors;
    }
    public GameHubAnonymousDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}
