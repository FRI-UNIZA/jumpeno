namespace Jumpeno.Client.Components;

public partial class ProfileModal {
    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;
    // Tabs:
    private PROFILE_TAB Tab = PROFILE_TAB.ACCOUNT;
    // Modals:
    private PasswordChangeModal PasswordChangeModalRef { get; set; } = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private CSSClass TabButtonClass(PROFILE_TAB tab) => new CSSClass("profile-tab-button").Set("active", Tab == tab);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await ModalRef.OpenLoading();
        Tab = PROFILE_TAB.ACCOUNT;
        var success = await HTTP.Try(Auth.LoadProfile);
        if (success) await ModalRef.FinishLoading();
        else await ModalRef.CloseLoading();
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private Action ChangeTab(PROFILE_TAB tab) => async () => {
        ModalRef.ScrollAreaRef.ScrollTo(0, 0);
        Tab = tab;
        StateHasChanged();
        await Task.Yield();
        ModalRef.ScrollAreaRef.InitScrollTo(0, 0);
    };
}
