namespace Jumpeno.Client.Components;

public partial class PasswordChangeModal
{
    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public Task Open() => ModalRef.Open();
}
