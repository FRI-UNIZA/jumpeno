namespace Jumpeno.Client.Components;

public partial class UserDropDown {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "user-dropdown";
    public const string ClassProfileImage = "user-dropdown-profile-image";
    public const string ClassIcon = "user-dropdown-icon";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private DropDown DropDownRef { get; set; } = null!;
    private ProfileModal ModalRef { get; set; } = null!;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task OpenProfile() => await ModalRef.Open();

    private async Task LogOut() {
        await PageLoader.Show(PageLoaderTask.Logout);
        AnimationHandler.CallOnTransitionEnd(Selector.ID(DropDownRef.IdButton), async () => {
            await HTTP.Try(Auth.LogOut);
            ActionHandler.PopFocus();
            await PageLoader.Hide(PageLoaderTask.Logout);
        });
    }
}
