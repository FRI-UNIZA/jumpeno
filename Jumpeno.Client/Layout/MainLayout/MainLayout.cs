namespace Jumpeno.Client.Layout;

public partial class MainLayout {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const string CLASS = "main-layout";
    private const string CLASS_NO_NAVIGATION = "no-navigation";
    private const string INERT_SELECTOR = $"#{WebDocument.ID}";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private NavMenu NavMenuRef = null!;
    private NavMenuMobile NavMenuMobileRef = null!;
    private CSSClass ComputeClass() {
        var c = new CSSClass(CLASS);
        if (!LayoutVM.NavigationDisplayed) c.Set(CLASS_NO_NAVIGATION);
        return c;
    }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly MainLayoutViewModel LayoutVM;
    public override void Notify() {
        NavMenuMobileRef?.Notify();
        base.Notify();
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public MainLayout() => LayoutVM = new MainLayoutViewModel(this);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void OnMobileMenuOpen() => ActionHandler.SetInert(INERT_SELECTOR);
    private void OnMobileMenuClose() => ActionHandler.RemoveInert(INERT_SELECTOR);
}
