namespace Jumpeno.Client.Services;

public static class ErrorHandler {
    // Show only message notification -----------------------------------------------------------------------------------------------------
    public static void Notify(string message) => Notification.Error(I18N.T(message, unsplit: true));
    public static void Notify(CoreException exception) => Notification.Error(I18N.T(exception.Message, unsplit: true));
    public static void Notify(CoreExceptionDTO exception) => Notification.Error(I18N.T(exception.Message, unsplit: true));
    public static void Notify(HTTPException exception) => Notification.Error(I18N.T(exception.Message, unsplit: true));

    // Show all error notifications -------------------------------------------------------------------------------------------------------
    public static void NotifyErrors(List<Error> errors, string? fallback = null) {
        if (errors.Count == 0 && fallback != null) Notify(fallback);
        foreach (var error in errors) {
            Notification.Error(I18N.T(error.Message, error.Values, unsplit: true));
        }
    }
    public static void NotifyErrors(CoreException exception, bool fallback = false)
        => NotifyErrors(exception.Errors, fallback ? exception.Message : null);
    public static void NotifyErrors(CoreExceptionDTO exception, bool fallback = false)
        => NotifyErrors(exception.Errors, fallback ? exception.Message : null);
    public static void NotifyErrors(HTTPException exception, bool fallback = false)
        => NotifyErrors(exception.Errors, fallback ? exception.Message : null);
    
    // Show message and all errors --------------------------------------------------------------------------------------------------------
    public static void NotifyAll(CoreException exception) {
        Notify(exception);
        NotifyErrors(exception);
    }
    public static void NotifyAll(CoreExceptionDTO exception) {
        Notify(exception);
        NotifyErrors(exception);
    }
    public static void NotifyAll(HTTPException exception) {
        Notify(exception);
        NotifyErrors(exception);
    }

    // Mark input fields with matching IDs ------------------------------------------------------------------------------------------------
    public static void MarkInputs(List<Error> errors) {
        foreach (var error in errors) {
            Input<object>.TrySetError(error);
        }
    }
    public static void MarkInputs(CoreException exception) => MarkInputs(exception.Errors);
    public static void MarkInputs(CoreExceptionDTO exception) => MarkInputs(exception.Errors);
    public static void MarkInputs(HTTPException exception) => MarkInputs(exception.Errors);

    // Complex exception handling ---------------------------------------------------------------------------------------------------------
    public static void Display(CoreException exception) {
        Notify(exception);
        MarkInputs(exception);
    }
    public static void Display(CoreExceptionDTO exception) {
        Notify(exception);
        MarkInputs(exception);
    }
    public static void Display(HTTPException exception) {
        Notify(exception);
        MarkInputs(exception);
    }
}
