namespace Jumpeno.Shared.Exceptions;

public class GameError : GameException {
    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameError(bool translated = true): base(translated) {}
    [JsonConstructor]
    public GameError(List<Error> errors, bool translated = true): base(errors, translated) {}
    public GameError(Exception? inner, bool translated = true): base(inner, translated) {}
    public GameError(List<Error> errors, Exception? inner, bool translated = true): base(errors, inner, translated) {}

    // Conversions ------------------------------------------------------------------------------------------------------------------------
    public override GameExceptionDTO DataTransferObject() {
        return new GameExceptionDTO(this);
    }
}
