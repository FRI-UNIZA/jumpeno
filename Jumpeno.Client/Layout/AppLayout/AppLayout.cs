namespace Jumpeno.Client.Layouts;

public partial class AppLayout {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const string CLASS = "main-layout";
    private const string CLASS_NO_NAVIGATION = "no-navigation";
    private const string INERT_SELECTOR = $"#{WebDocument.ID}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private NavMenu NavMenuRef = null!;
    private NavMenuMobile NavMenuMobileRef = null!;
    private CSSClass ComputeClass() {
        var c = ComputeClass(CLASS);
        if (!LayoutVM.NavigationDisplayed) c.Set(CLASS_NO_NAVIGATION);
        return c;
    }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly AppLayoutVM LayoutVM = new();
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnComponentInitializedAsync() => await Auth.Register(this);
    protected override bool ShouldComponentRender() => Auth.NotFreezed(this);
    protected override async ValueTask OnComponentDisposeAsync() => await Auth.Register(this);

    // Notification -----------------------------------------------------------------------------------------------------------------------
    private bool Key { get; set; } = false;
    private bool PageKey { get; set; } = false;
    private void NotifyState() { Layout.Current.Notify(); StateHasChanged(); }
    private void NotifyPage() { PageKey = !PageKey; NotifyState(); }
    private void NotifyAll() { Key = !Key; NotifyState(); }
    public static void Notify(NOTIFY notify) {
        switch (notify) {
            case NOTIFY.STATE: Instance().NotifyState(); break;
            case NOTIFY.PAGE: Instance().NotifyPage(); break;
            case NOTIFY.ALL: Instance().NotifyAll(); break;
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void OnMobileMenuOpen() => ActionHandler.SetInert(INERT_SELECTOR);
    private void OnMobileMenuClose() => ActionHandler.RemoveInert(INERT_SELECTOR);
}
