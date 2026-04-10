namespace Jumpeno.Client.Models;

public record GameMapDTO : IValidable<GameMapDTO> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    /// <summary>Map ID</summary>
    public required int ID { get; init; }
    
    // Validation -------------------------------------------------------------------------------------------------------------------------
    public List<Error> Validate() => MapValidator.ValidateID(ID, nameof(ID));
    public GameMapDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? Exceptions.VALUES);
}
