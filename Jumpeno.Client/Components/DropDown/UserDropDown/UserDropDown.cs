namespace Jumpeno.Client.Components;

public partial class UserDropDown : IAsyncDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "user-dropdown";
    public const string CLASS_PROFILE_IMAGE = "user-dropdown-profile-image";
    public const string CLASS_ICON = "user-dropdown-icon";
    // Duration:
    public static readonly int TRANSITION_BUTTON = THEME.DEFAULT.TRANSITION_SLOW;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private DropDown Ref { get; set; } = null!;
    private UserProfileModal UserProfileModalRef { get; set; } = null!;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnInitializedAsync() => await Auth.AddUpdateListener(StateHasChanged);
    public async ValueTask DisposeAsync() => await Auth.RemoveUpdateListener(StateHasChanged);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task OpenProfile() => await UserProfileModalRef.Open();

    public static async Task LogOut() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGOUT);
        await Task.Delay(TRANSITION_BUTTON);
        await HTTP.Try(Auth.LogOut);
        ActionHandler.PopFocus();
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGOUT);
    }
}
