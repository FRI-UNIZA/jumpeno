namespace Jumpeno.Server.Utils;

public class RefreshCleaner(IServiceScopeFactory scopeFactory) : Cron {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    protected override TimeSpan Interval => TimeSpan.FromMinutes(ServerSettings.Schedule.RefreshCleaner.Minutes);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    protected override async Task RunAsync(CancellationToken stoppingToken) {
        using var scope = scopeFactory.CreateScope();
        await scope.ServiceProvider.GetRequiredService<RefreshService>().DeleteExpired();
    }
}
