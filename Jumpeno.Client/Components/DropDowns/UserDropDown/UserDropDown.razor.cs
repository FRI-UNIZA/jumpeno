namespace Jumpeno.Client.Components;

public partial class UserDropDown {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "user-dropdown";
    public const string CLASS_PROFILE_IMAGE = "user-dropdown-profile-image";
    public const string CLASS_ICON = "user-dropdown-icon";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private DropDown DropDownRef { get; set; } = null!;
    private ProfileModal ModalRef { get; set; } = null!;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task OpenProfile() => await ModalRef.Open();

    private async Task LogOut() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGOUT);
        AnimationHandler.CallOnTransitionEnd(Selector.ID(DropDownRef.ID_BUTTON), async () => {
            await HTTP.Try(Auth.LogOut);
            ActionHandler.PopFocus();
            await PageLoader.Hide(PAGE_LOADER_TASK.LOGOUT);
        });
    }
}
