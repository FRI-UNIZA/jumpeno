namespace Jumpeno.Client.Models;

public record GamePlayerControlDTO : IValidable<GamePlayerControlDTO> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    /// <summary>Game code</summary>
    public required string Code { get; init; }
    /// <summary>Player name</summary>
    public required string Name { get; init; }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public List<Error> Validate() {
        List<Error> errors = [];
        errors.AddRange(GameValidator.ValidateCode(Code, nameof(Code)));
        errors.AddRange(UserValidator.ValidateName(Name, checkUnknown: true, nameof(Name)));
        return errors;
    }
    public GamePlayerControlDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? Exceptions.Values);
}
