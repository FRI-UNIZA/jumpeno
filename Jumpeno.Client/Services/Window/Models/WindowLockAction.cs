namespace Jumpeno.Client.Models;

public class WindowLockAction(
    TaskCompletionSource tcs, dynamic action,
    Exception? exception = null, object? response = null
) {
    public TaskCompletionSource TCS { get; set; } = tcs;
    public dynamic Action { get; set; } = action;
    public Exception? Exception { get; set; } = exception;
    public object? Response { get; set; } = response;
}
