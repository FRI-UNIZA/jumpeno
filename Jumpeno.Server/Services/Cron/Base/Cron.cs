namespace Jumpeno.Server.Utils;

public abstract class Cron : BackgroundService {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected abstract TimeSpan Interval { get; }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            try {
                await RunAsync(stoppingToken);
            } catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                break;
            } catch {}

            await Task.Delay(Interval, stoppingToken);
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    protected abstract Task RunAsync(CancellationToken stoppingToken);
}
