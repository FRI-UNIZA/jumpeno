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
    private bool IsError = false;

    private async Task LoadModal() {
        await HTTP.Try(async () => {
            try {
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
                // 1.5) Display success:
                IsError = false;
                StateHasChanged();
            } catch {
                // 2.1) Display error:
                IsError = true;
                StateHasChanged();
                // 2.2) Rethrow:
                throw;
            }
        });
    }

    private void ClearModal() => Credentials = [];

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await ModalRef.OpenLoading();
        await LoadModal();
        await ModalRef.FinishLoading();
    }

    private async Task Refresh() {
        await PageLoader.Show(PAGE_LOADER_TASK.ADMIN_CREDENTIALS);
        await LoadModal();
        await PageLoader.Hide(PAGE_LOADER_TASK.ADMIN_CREDENTIALS);
        if (!IsError) ActionHandler.SetFocus(ModalRef.ID_DIALOG);
    }
}
