namespace Jumpeno.Shared.Exceptions;

public class CoreExceptionDTO {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public EXCEPTION_TYPE Type { get; private set; }
    public int Code { get; private set; }
    public string Message { get; private set; }
    public List<Error> Errors { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    protected CoreExceptionDTO(EXCEPTION_TYPE type, int code, string message, List<Error> errors) {
        Type = type;
        Code = code;
        Message = message;
        Errors = errors;
    }
    public CoreExceptionDTO(CoreException exception) : this(
        exception is CoreError ? EXCEPTION_TYPE.ERROR : EXCEPTION_TYPE.EXCEPTION,
        exception.Code,
        exception.Message,
        exception.Errors
    ) {}
}
