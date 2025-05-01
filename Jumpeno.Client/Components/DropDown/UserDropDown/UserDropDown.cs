namespace Jumpeno.Client.Components;

public partial class UserDropDown {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "user-dropdown";
    public const string CLASS_PROFILE_IMAGE = "user-dropdown-profile-image";
    public const string CLASS_ICON = "user-dropdown-icon";
    // Duration:
    public static readonly int TRANSITION_BUTTON = THEME.DEFAULT.TRANSITION_SLOW;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private ProfileModal ModalRef { get; set; } = null!;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task OpenProfile() => await ModalRef.Open();

    public static async Task LogOut() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGOUT);
        await Task.Delay(TRANSITION_BUTTON);
        await HTTP.Try(Auth.LogOut);
        ActionHandler.PopFocus();
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGOUT);
    }
}
