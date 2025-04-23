namespace Jumpeno.Server.Services;

public static class CronService {
    // Crons ------------------------------------------------------------------------------------------------------------------------------
    public static readonly ActivationCleaner ActivationCleaner = new();
    public static readonly RefreshCleaner RefreshCleaner = new();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Start() {
        ActivationCleaner.Start();
        RefreshCleaner.Start();
    }
}
