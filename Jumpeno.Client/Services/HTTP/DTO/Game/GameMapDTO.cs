namespace Jumpeno.Client.Models;

public record GameMapDTO(
    int ID
) : IValidable<GameMapDTO> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    /// <summary>Map ID</summary>
    public required int ID { get; init; } = ID;
    
    // Validation -------------------------------------------------------------------------------------------------------------------------
    public List<Error> Validate() {
        var errors = Checker.ValidateUndefined(ID, nameof(ID));
        if (ID < 0) errors.Add(ERROR.FORMAT.SetID(nameof(ID)));
        return errors;
    }
    public GameMapDTO Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.VALUES);
}
