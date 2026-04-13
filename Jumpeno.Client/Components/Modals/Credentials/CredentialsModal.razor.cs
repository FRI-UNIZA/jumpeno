namespace Jumpeno.Client.Components;

public partial class CredentialsModal {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string FIX_URL = API.BASE.ADMIN_DB_CREDENTIALS;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required OneOf<string, List<string>> Label { get; set; }
    [Parameter]
    public required string URL { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Data -------------------------------------------------------------------------------------------------------------------------------
    private string[] Credentials = [];

    private async Task<bool> LoadModal() {
        return await HTTP.Try(async () => {
            // 1.1) Load credentials:
            var response = await HTTP.Get<MessageDTOR>(URL);
            // 1.2) Assert response:
            response.Body.Assert();
            // 1.3) Split values:
            Credentials = response.Body.Message.Split(';', StringSplitOptions.RemoveEmptyEntries);
            // 1.4) Fix values:
            if (URL == FIX_URL) {
                for (int i = 0; i < Credentials.Length; i++) {
                    Credentials[i] = Credentials[i].Replace("localhost", "database");
                }
            }
        });
    }

    private void ClearModal() => Credentials = [];

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await ModalRef.OpenLoading();
        if (await LoadModal()) await ModalRef.FinishLoading();
        else await ModalRef.CloseLoading();
    }
}
