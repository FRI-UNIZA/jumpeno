namespace Jumpeno.Shared.Exceptions;

public class GameError: GameException {
    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameError(): base() {}
    [JsonConstructor]
    public GameError(List<Error> errors): base(errors) {}
    public GameError(Exception? inner): base(inner) {}
    public GameError(List<Error> errors, Exception? inner): base(errors, inner) {}

    // Conversions ------------------------------------------------------------------------------------------------------------------------
    public override GameExceptionDTO DataTransferObject() {
        return new GameExceptionDTO(this);
    }
}
