namespace Jumpeno.Client.Components;

public partial class AdminDropDown {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "admin-dropdown";
    public const string CLASS_PROFILE_IMAGE = "admin-dropdown-profile-image";
    public const string CLASS_ICON = "admin-dropdown-icon";
    // Duration:
    public static readonly int TRANSITION_BUTTON = THEME.DEFAULT.TRANSITION_SLOW;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task OpenSettings() => await Navigator.NavigateTo(I18N.Link<AdminPage>());

    public static async Task LogOut() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGOUT);
        await Task.Delay(TRANSITION_BUTTON);
        await HTTP.Try(Auth.LogOut);
        ActionHandler.PopFocus();
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGOUT);
    }
}
