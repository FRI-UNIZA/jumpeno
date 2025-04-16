namespace Jumpeno.Shared.Models;

// NOTE: CoreError is weak CoreException (does not mean socket connection should close)
public class CoreError : CoreException {
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    protected CoreError(int code, string message, List<Error> errors) : base(code, message, errors) {}
    public CoreError() : base() {}
    public CoreError(Error error) : base(error) {}
    public CoreError(List<Error> errors) : base(errors) {}

    // Data Transfer Object ---------------------------------------------------------------------------------------------------------------
    public override CoreExceptionDTO DTO => new(this);
}
