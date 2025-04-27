namespace Jumpeno.Client.Components;

public partial class UserProfileModal {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Data -------------------------------------------------------------------------------------------------------------------------------
    private bool IsError = false;

    private async Task LoadModal() {
        await HTTP.Try(async () => {
            try {
                // 1.1) Load profile:
                await Auth.LoadProfile();
                // 1.2) Display success:
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

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await ModalRef.OpenLoading();
        await LoadModal();
        await ModalRef.FinishLoading();
    }

    private async Task Refresh() {
        await PageLoader.Show(PAGE_LOADER_TASK.PROFILE);
        await LoadModal();
        await PageLoader.Hide(PAGE_LOADER_TASK.PROFILE);
        // NOTE: Focus modal from refresh button:
        if (!IsError) ActionHandler.SetFocus(ModalRef.ID_DIALOG);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task SendActivationLink() {
        await PageLoader.Show(PAGE_LOADER_TASK.ACTIVATION);
        await HTTP.Try(async () => {
            var result = await HTTP.Post<MessageDTOR>(API.BASE.USER_SEND_ACTIVATION);
            Notification.Success(result.Body.Message);
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.ACTIVATION);
    }
}
