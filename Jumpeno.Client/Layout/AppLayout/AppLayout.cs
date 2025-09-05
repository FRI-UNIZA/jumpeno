namespace Jumpeno.Client.Layouts;

public partial class AppLayout {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const string CLASS = "main-layout";
    private const string CLASS_NO_NAVIGATION = "no-navigation";
    private const string INERT_SELECTOR = $"#{WebDocument.ID}";
    // Cascade:
    public const string CASCADE_APP_LAYOUT = $"{nameof(AppLayout)}.{nameof(CASCADE_APP_LAYOUT)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private NavMenu NavMenuRef = null!;
    private NavMenuMobile NavMenuMobileRef = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(CLASS, Base)
        .Set(CLASS_NO_NAVIGATION, !LayoutVM.NavigationDisplayed);
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
    private static void OnMobileMenuOpen() => ActionHandler.SetInert(INERT_SELECTOR);
    private static void OnMobileMenuClose() => ActionHandler.RemoveInert(INERT_SELECTOR);
}
