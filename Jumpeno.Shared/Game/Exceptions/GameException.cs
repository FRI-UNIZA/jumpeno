namespace Jumpeno.Shared.Exceptions;

public class GameException: Exception {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string DEFAULT_MESSAGE() => I18N.T("Something went wrong.");

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public List<Error> Errors { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameException(): base(DEFAULT_MESSAGE()) { Errors = []; }
    [JsonConstructor]
    public GameException(List<Error> errors): base(DEFAULT_MESSAGE()) { Errors = errors; }
    public GameException(Exception? inner): base(DEFAULT_MESSAGE(), inner) { Errors = []; }
    public GameException(List<Error> errors, Exception? inner): base(DEFAULT_MESSAGE(), inner) { Errors = errors; }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void Add(Error error) {
        Errors.Add(error);
    }

    public void Add(List<Error> errors) {
        Errors.AddRange(errors);
    }

    public bool HasErrors() {
        return Errors.Count > 0;
    }

    // Conversions ------------------------------------------------------------------------------------------------------------------------
    public GameExceptionDTO DataTransferObject() {
        return new GameExceptionDTO(Errors);
    }
}

public class GameExceptionDTO {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Message { get; private set; }
    public List<Error> Errors { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private GameExceptionDTO(string message, List<Error> errors) {
        Message = message;
        Errors = errors;
    }
    public GameExceptionDTO(List<Error> errors): this(GameException.DEFAULT_MESSAGE(), errors) {}
}
