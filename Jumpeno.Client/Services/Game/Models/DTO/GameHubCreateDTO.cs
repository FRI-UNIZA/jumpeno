namespace Jumpeno.Client.Models;

public record GameHubCreateDTO(
    // Game:
    string? Code,
    string GameName,
    int? Map,
    bool Anonyms,
    byte Rounds,
    byte Capacity,
    DisplayMode DisplayMode,
    GameMode GameMode,
    // Player:
    string AccessToken,
    DeviceType Device
) : IValidable<GameHubCreateDTO> {
    public List<Error> Validate() {
        var errors = Code != null ? GameValidator.ValidateCode(Code, nameof(Code)) : [];
        errors.AddRange(GameValidator.ValidateName(GameName, nameof(GameName)));
        if (Map != null) errors.AddRange(MapValidator.ValidateID(Map, nameof(Map)));
        errors.AddRange(GameValidator.ValidateAnonyms(Anonyms, nameof(Anonyms)));
        errors.AddRange(GameValidator.ValidateRounds(Rounds, nameof(Rounds)));
        errors.AddRange(GameValidator.ValidateCapacity(Capacity, nameof(Capacity)));
        errors.AddRange(GameValidator.ValidateDisplayMode(DisplayMode, nameof(DisplayMode)));
        errors.AddRange(GameValidator.ValidateGameMode(GameMode, nameof(GameMode)));
        errors.AddRange(TokenValidator.ValidateToken(AccessToken, nameof(AccessToken)));
        errors.AddRange(UserValidator.ValidateDeviceType(Device, nameof(Device)));
        return errors;
    }
    public GameHubCreateDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? Exceptions.VALUES);
}
