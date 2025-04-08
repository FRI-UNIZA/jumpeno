namespace Jumpeno.Server.Utils;

#pragma warning disable CS4014
#pragma warning disable CS1998

public class Cron {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public virtual int INTERVAL { get; } // ms

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Start() => RunLoop();

    private async Task RunLoop() {
        while (true) {
            try { Run(); } catch {}
            try { await RunAsync(); } catch {}
            await Task.Delay(INTERVAL);
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    protected virtual void Run() {}
    protected virtual async Task RunAsync() {}
}
