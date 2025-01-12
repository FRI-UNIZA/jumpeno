namespace Jumpeno.Shared.Exceptions;

public class CoreException : Exception {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int DEFAULT_CODE = 500;
    public const string DEFAULT_MESSAGE = "Something went wrong.";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int Code { get; set; } = DEFAULT_CODE;
    public List<Error> Errors { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    protected CoreException(int code, string message, List<Error> errors) : base(message) { Code = code; Errors = errors; }
    public CoreException() : base(DEFAULT_MESSAGE) => Errors = [];
    public CoreException(Error error) : this([error]) {}
    public CoreException(List<Error> errors) : base(DEFAULT_MESSAGE) => Errors = errors;

    // Message ----------------------------------------------------------------------------------------------------------------------------
    public CoreException SetCode(int code) { Code = code; return this; }
    public CoreException SetMessage(string message) { Reflex.SetField(typeof(Exception), this, "_message", message); return this; }

    // Errors -----------------------------------------------------------------------------------------------------------------------------
    public void Add(Error error) => Errors.Add(error);
    public void Add(List<Error> errors) => Errors.AddRange(errors);
    public bool HasErrors => Errors.Count > 0;

    // Inner exception --------------------------------------------------------------------------------------------------------------------
    public void SetInner(Exception inner) => Reflex.SetField(typeof(Exception), this, "_innerException", inner);

    // Data Transfer Object ---------------------------------------------------------------------------------------------------------------
    public virtual CoreExceptionDTO DTO => new(this);
}
