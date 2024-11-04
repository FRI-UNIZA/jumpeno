namespace Jumpeno.Shared.Exceptions;

public class GameExceptionDTO {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GAME_EXCEPTION_TYPE Type { get; private set; }
    public string Message { get; private set; }
    public List<Error> Errors { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private GameExceptionDTO(GAME_EXCEPTION_TYPE type, string message, List<Error> errors) {
        Type = type;
        Message = message;
        Errors = errors;
    }
    public GameExceptionDTO(GameException exception): this(
        exception is GameError ? GAME_EXCEPTION_TYPE.ERROR : GAME_EXCEPTION_TYPE.EXCEPTION,
        GameException.DEFAULT_MESSAGE(),
        exception.Errors
    ) {}
}
