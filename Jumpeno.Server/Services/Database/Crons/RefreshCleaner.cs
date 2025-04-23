namespace Jumpeno.Server.Utils;

public class RefreshCleaner : Cron {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public override int INTERVAL => From.MinToMS(ServerSettings.Schedule.RefreshCleaner.Minutes); // ms

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    protected override async Task RunAsync() {
        await DB.UseServerContext(RefreshEntity.DeleteExpired);
    }
}
