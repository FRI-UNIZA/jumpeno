namespace Jumpeno.Client.Services;

public static class ErrorHandler {
    // Translations -----------------------------------------------------------------------------------------------------------------------
    private static string T(string key, bool translate) => translate ? I18N.T(key, unsplit: true) : key;
    private static string T(string key, Dictionary<string, string> values, bool translate) => translate ? I18N.T(key, values, unsplit: true) : key;

    // Show only message notification -----------------------------------------------------------------------------------------------------
    public static void Notify(string message, bool translate = false) => Notification.Error(T(message, translate));
    public static void Notify(CoreException exception, bool translate = false) => Notification.Error(T(exception.Message, translate));
    public static void Notify(CoreExceptionDTO exception, bool translate = false) => Notification.Error(T(exception.Message, translate));
    public static void Notify(HTTPException exception, bool translate = false) => Notification.Error(T(exception.Message, translate));

    // Show all error notifications -------------------------------------------------------------------------------------------------------
    public static void NotifyErrors(List<Error> errors, string? fallback = null, bool translate = false) {
        if (errors.Count == 0 && fallback != null) Notify(fallback, translate);
        foreach (var error in errors) {
            Notification.Error(T(error.Message, error.Values, translate));
        }
    }
    public static void NotifyErrors(CoreException exception, bool fallback = false, bool translate = false)
        => NotifyErrors(exception.Errors, fallback ? exception.Message : null, translate);
    public static void NotifyErrors(CoreExceptionDTO exception, bool fallback = false, bool translate = false)
        => NotifyErrors(exception.Errors, fallback ? exception.Message : null, translate);
    public static void NotifyErrors(HTTPException exception, bool fallback = false, bool translate = false)
        => NotifyErrors(exception.Errors, fallback ? exception.Message : null, translate);
    
    // Show message and all errors --------------------------------------------------------------------------------------------------------
    public static void NotifyAll(CoreException exception, bool translate = false) {
        Notify(exception, translate);
        NotifyErrors(exception, translate);
    }
    public static void NotifyAll(CoreExceptionDTO exception, bool translate = false) {
        Notify(exception, translate);
        NotifyErrors(exception, translate);
    }
    public static void NotifyAll(HTTPException exception, bool translate = false) {
        Notify(exception, translate);
        NotifyErrors(exception, translate);
    }

    // Mark input fields with matching IDs ------------------------------------------------------------------------------------------------
    public static void MarkInputs(List<Error> errors, bool translate = false) {
        foreach (var error in errors) {
            Input<object>.TrySetError(error, translate);
        }
    }
    public static void MarkInputs(CoreException exception, bool translate = false) => MarkInputs(exception.Errors, translate);
    public static void MarkInputs(CoreExceptionDTO exception, bool translate = false) => MarkInputs(exception.Errors, translate);
    public static void MarkInputs(HTTPException exception, bool translate = false) => MarkInputs(exception.Errors, translate);

    // Complex exception handling ---------------------------------------------------------------------------------------------------------
    public static void Display(CoreException exception, bool translate = false) {
        Notify(exception, translate);
        MarkInputs(exception, translate);
    }
    public static void Display(CoreExceptionDTO exception, bool translate = false) {
        Notify(exception, translate);
        MarkInputs(exception, translate);
    }
    public static void Display(HTTPException exception, bool translate = false) {
        Notify(exception, translate);
        MarkInputs(exception, translate);
    }
}
