namespace Jumpeno.Shared.Exceptions;

public class GameExceptionDTO {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GAME_EXCEPTION_TYPE Type { get; private set; }
    public bool Translated { get; private set; }
    public string Message { get; private set; }
    public List<Error> Errors { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private GameExceptionDTO(GAME_EXCEPTION_TYPE type, bool translated, string message, List<Error> errors) {
        Type = type;
        Translated = translated;
        Message = message;
        Errors = errors;
    }
    public GameExceptionDTO(GameException exception): this(
        exception is GameError ? GAME_EXCEPTION_TYPE.ERROR : GAME_EXCEPTION_TYPE.EXCEPTION,
        exception.Translated,
        exception.Message,
        exception.Errors
    ) {}
}
