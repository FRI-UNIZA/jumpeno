namespace Jumpeno.Client.Models;

public class HTTPToken {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool IsCancelled { get; private set; } = false;
    public CancellationTokenSource? Token { get; set; } = null;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Cancel() {
        IsCancelled = true;
        try { Token?.Cancel(); } catch {}
    }
}
