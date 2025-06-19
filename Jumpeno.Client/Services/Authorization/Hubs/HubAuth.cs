namespace Jumpeno.Client.Services;

public class HubAuth {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Func<Task>? Request = null;
    private Func<Task>? LastRequest = null;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Start connect request.</summary>
    /// <param name="request">Request method</param>
    public void Start(Func<Task> request) => Request = request;

    /// <summary>Releases resources.</summary>
    public void OnSuccess() {
        Request = null;
        LastRequest = null;
    }

    /// <summary>
    ///     Handles expired token refresh on exception or returns silently.
    ///     Exception is rethrown if handled.
    /// </summary>
    /// <param name="exception">Thrown exception</param>
    /// <param name="dispose">Function to dispose Hub</param>
    public async Task OnError(AppException exception, Func<Task> dispose) {
        if (exception.Code != CODE.NOT_AUTHENTICATED) return;
        if (Request == null) return;
        if (LastRequest != Request) {
            await dispose();
            await Auth.Refresh(1);
            LastRequest = Request;
            await Request();
        } else {
            await Auth.Refresh(2);
        }
        throw exception;
    }
}
