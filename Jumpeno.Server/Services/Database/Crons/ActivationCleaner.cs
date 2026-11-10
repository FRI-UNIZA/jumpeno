namespace Jumpeno.Server.Utils;

public class ActivationCleaner(IServiceScopeFactory scopeFactory) : Cron {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    protected override TimeSpan Interval => TimeSpan.FromMinutes(ServerSettings.Schedule.ActivationCleaner.Minutes);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    protected override async Task RunAsync(CancellationToken stoppingToken) {
        using var scope = scopeFactory.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ActivationService>().DeleteExpired();
    }
}
