namespace Jumpeno.Shared.Exceptions;

public class GameException : Exception {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string DEFAULT_MESSAGE() => "Something went wrong.";
    public static string DEFAULT_MESSAGE_TRANSLATED() => I18N.T(DEFAULT_MESSAGE());

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool Translated { get; private set; }
    public List<Error> Errors { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameException(bool translated = true): base(translated ? DEFAULT_MESSAGE_TRANSLATED() : DEFAULT_MESSAGE())
    { Translated = translated; Errors = []; }
    [JsonConstructor]
    public GameException(List<Error> errors, bool translated = true): base(translated ? DEFAULT_MESSAGE_TRANSLATED() : DEFAULT_MESSAGE())
    { Translated = translated; Errors = errors; }
    public GameException(Exception? inner, bool translated = true): base(translated ? DEFAULT_MESSAGE_TRANSLATED() : DEFAULT_MESSAGE(), inner)
    { Translated = translated; Errors = []; }
    public GameException(List<Error> errors, Exception? inner, bool translated = true): base(translated ? DEFAULT_MESSAGE_TRANSLATED() : DEFAULT_MESSAGE(), inner)
    { Translated = translated; Errors = errors; }

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
    public virtual GameExceptionDTO DataTransferObject() {
        return new GameExceptionDTO(this);
    }
}
