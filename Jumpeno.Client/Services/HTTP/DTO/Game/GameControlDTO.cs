namespace Jumpeno.Client.Models;

public record GameControlDTO : IValidable<GameControlDTO> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    /// <summary>Game code</summary>
    public required string Code { get; init; }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public List<Error> Validate() => GameValidator.ValidateCode(Code, nameof(Code));
    public GameControlDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? Exceptions.Values);
}
