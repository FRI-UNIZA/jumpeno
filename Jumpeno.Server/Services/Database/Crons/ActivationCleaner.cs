namespace Jumpeno.Server.Utils;

public class ActivationCleaner : Cron {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public override int INTERVAL => From.MinToMS(ServerSettings.Schedule.ActivationCleaner.Minutes); // ms

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    protected override async Task RunAsync() {
        await DB.UseServerContext(ActivationEntity.DeleteExpired);
    }
}
