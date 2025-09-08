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
    ///     Handles expired token refresh on request exception.
    ///     If refresh fails, exception is returned to display errors.
    ///     Exception is rethrown if handled properly to finish error handling.
    /// </summary>
    /// <param name="exceptionDTO">Thrown exception DTO</param>
    /// <param name="dispose">Function to dispose Hub</param>
    /// <returns>Refresh exception if occured.</returns>
    public async Task<AppException?> OnError(AppExceptionDTO exceptionDTO, Func<Task> dispose) {
        if (exceptionDTO.Code != CODE.NOT_AUTHENTICATED) return null;
        if (Request == null) return null;
        if (LastRequest != Request) {
            await dispose();
            try { await HTTP.Sync(async () => await Auth.Refresh(1)); }
            catch (AppException e) { return e; }
            LastRequest = Request;
            await Request();
        } else {
            await HTTP.Sync(async () => await Auth.Refresh(2));
        }
        throw exceptionDTO.Exception;
    }
}
