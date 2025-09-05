namespace Jumpeno.Client.Components;

public partial class AdminDropDown {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "admin-dropdown";
    public const string CLASS_PROFILE_IMAGE = "admin-dropdown-profile-image";
    public const string CLASS_ICON = "admin-dropdown-icon";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private DropDown DropDownRef { get; set; } = null!;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private static async Task OpenSettings() => await Navigator.NavigateTo(I18N.Link<AdminPage>());

    private async Task LogOut() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGOUT);
        AnimationHandler.CallOnTransitionEnd(Selector.ID(DropDownRef.ID_BUTTON), async () => {
            await HTTP.Try(Auth.LogOut);
            ActionHandler.PopFocus();
            await PageLoader.Hide(PAGE_LOADER_TASK.LOGOUT);
        });
    }
}
