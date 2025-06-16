namespace Jumpeno.Client.Pages;

public partial class AdminPage {
    public const string ROUTE_EN = "/en/admin";
    public const string ROUTE_SK = "/sk/admin";
    public static readonly ROLE[] ROLES = [ROLE.ADMIN];

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CredentialsModal DBCredentialsModalRef { get; set; } = null!;
    private CredentialsModal EmailPasswordModalRef { get; set; } = null!;
    private CredentialsModal EmailBackupKeysModalRef { get; set; } = null!;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task OpenDBCredentials() => await DBCredentialsModalRef.Open();
    public async Task OpenEmailPassword() => await EmailPasswordModalRef.Open();
    public async Task OpenEmailBackupKeys() => await EmailBackupKeysModalRef.Open();
}
