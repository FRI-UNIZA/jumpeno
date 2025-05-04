namespace Jumpeno.Client.Services;

public static class ErrorHandler {
    // Translation ------------------------------------------------------------------------------------------------------------------------
    private static string T(TInfo info) => I18N.T(info, unsplit: true);

    // Show only message notification -----------------------------------------------------------------------------------------------------
    public static void Notify(TInfo info) => Notification.Error(T(info));
    public static void Notify(AppException exception) => Notify(exception.Info);

    // Show all error notifications -------------------------------------------------------------------------------------------------------
    public static void NotifyErrors(List<Error> errors) {
        foreach (var error in errors) {
            Notification.Error(T(error.Info));
        }
    }
    public static void NotifyErrors(AppException exception) => NotifyErrors(exception.Errors);

    // Mark input fields with matching IDs ------------------------------------------------------------------------------------------------
    public static void MarkInputs(string? form, List<Error> errors) {
        if (form == null || form == "") return;
        foreach (var error in errors) {
            Input<object>.TrySetError(form, error);
        }
    }
    public static void MarkInputs(string? form, AppException exception) => MarkInputs(form, exception.Errors);

    // Show message and all errors --------------------------------------------------------------------------------------------------------
    public static void NotifyAll(AppException exception) {
        Notify(exception);
        NotifyErrors(exception);
    }

    // Complex exception handling ---------------------------------------------------------------------------------------------------------
    public static void Display(AppException exception, string? form = null) {
        Notify(exception);
        MarkInputs(form, exception);
    }
}
