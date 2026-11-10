namespace Jumpeno.Client.Components;

public partial class AdminDropDown {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "admin-dropdown";
    public const string ClassProfileImage = "admin-dropdown-profile-image";
    public const string ClassIcon = "admin-dropdown-icon";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private DropDown DropDownRef { get; set; } = null!;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private static async Task OpenSettings() => await Navigator.NavigateTo(I18N.Link<AdminPage>());

    private async Task LogOut() {
        await PageLoader.Show(PageLoaderTask.Logout);
        AnimationHandler.CallOnTransitionEnd(Selector.ID(DropDownRef.IdButton), async () => {
            await HTTP.Try(Auth.LogOut);
            ActionHandler.PopFocus();
            await PageLoader.Hide(PageLoaderTask.Logout);
        });
    }
}
