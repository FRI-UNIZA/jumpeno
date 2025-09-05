namespace Jumpeno.Client.Components;

public partial class ProfileModal {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await ModalRef.OpenLoading();
        var success = await HTTP.Try(Auth.LoadProfile);
        if (success) await ModalRef.FinishLoading();
        else await ModalRef.CloseLoading();
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
