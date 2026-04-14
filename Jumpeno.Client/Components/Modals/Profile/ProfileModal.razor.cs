namespace Jumpeno.Client.Components;

public partial class ProfileModal {
    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;
    // Tabs:
    private ProfileTabType Tab = ProfileTabType.Account;
    // Modals:
    private PasswordChangeModal PasswordChangeModalRef { get; set; } = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private CSSClass TabButtonClass(ProfileTabType tab) => new CSSClass("profile-tab-button").Set("active", Tab == tab);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await ModalRef.OpenLoading();
        Tab = ProfileTabType.Account;
        var success = await HTTP.Try(Auth.LoadProfile);
        if (success) await ModalRef.FinishLoading();
        else await ModalRef.CloseLoading();
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private Action ChangeTab(ProfileTabType tab) => async () => {
        ModalRef.ScrollAreaRef.ScrollTo(0, 0);
        Tab = tab;
        StateHasChanged();
        await Task.Yield();
        ModalRef.ScrollAreaRef.InitScrollTo(0, 0);
    };
}
