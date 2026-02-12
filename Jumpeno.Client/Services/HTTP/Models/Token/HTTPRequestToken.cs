namespace Jumpeno.Client.Models;

public class HTTPRequestToken : IAsyncDisposable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private HTTPToken Token = new();
    private readonly LockerSlim Lock = new();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    /// <summary>Cancels token and disposes it's lock.</summary>
    /// <returns>Task to await</returns>
    public async ValueTask DisposeAsync() => await Lock.TryExclusive(() => {
        Token.Cancel();
        Lock.DisposeUnsafe();
        GC.SuppressFinalize(this); 
    });

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Resets current token and returns new one for next request.</summary>
    /// <returns>Token or null if disposed (request logic should be terminated in that case)</returns>
    public Task<HTTPToken?> Reset() => Lock.TryExclusive(
        () => {
            // 1) Cancel token:
            Token.Cancel();
            // 2) Create new:
            Token = new();
            // 3) Return token:
            return Token;
            // 4) Null if disposed:
        }, null
    );
}
