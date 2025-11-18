namespace Jumpeno.Client.Components;

public partial class ProfileModal {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private TABS_POSITION TabsPosition = TABS_POSITION.LEFT;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly DotNetObjectReference<ProfileModal> ObjectReference;
    private Modal ModalRef = null!;
    // Tabs:
    private AccountTab AccountRef { get; set; } = null!;
    private SocialsTab SocialsRef { get; set; } = null!;
    private AvatarTab AvatarRef { get; set; } = null!;
    // Modals:
    private PasswordChangeModal PasswordChangeModalRef { get; set; } = null!;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ProfileModal() => ObjectReference = DotNetObjectReference.Create(this);

    override protected async Task OnComponentInitializedAsync()
    {
        if (AppEnvironment.IsServer) return;
        //NOTE: Adjust tab position based on initial window size even before any resize occurs (only once)
        SetModalSizeBasedOnViewport();
        await Window.AddResizeEventListener(ObjectReference, JS_OnResize);
        SetModalSizeBasedOnViewport();
    }

    protected override async ValueTask OnComponentDisposeAsync()
    {
        await Window.RemoveResizeEventListener(ObjectReference, JS_OnResize);
        ObjectReference.Dispose();
    }

    // Resize -----------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public void JS_OnResize(WindowResizeEvent e)
    {
        if (e.Width < THEME.DEFAULT.BREAKPOINT_MOBILE && TabsPosition != TABS_POSITION.TOP)
        {
            TabsPosition = TABS_POSITION.TOP;
            StateHasChanged();
        }
        else if (e.Width >= THEME.DEFAULT.BREAKPOINT_MOBILE && TabsPosition != TABS_POSITION.LEFT)
        {
            TabsPosition = TABS_POSITION.LEFT;
            StateHasChanged();
        }
    }

    private void SetModalSizeBasedOnViewport()
    {
        TabsPosition = Window.GetSize().Width < THEME.DEFAULT.BREAKPOINT_MOBILE ? TABS_POSITION.TOP : TABS_POSITION.LEFT;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        await ModalRef.OpenLoading();
        var success = await HTTP.Try(Auth.LoadProfile);
        if (success) await ModalRef.FinishLoading();
        else await ModalRef.CloseLoading();
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private void ResetForms()
    {
        AccountRef.ResetForm();
        SocialsRef.ResetForm();
        AvatarRef.ResetForm();
    }
}
