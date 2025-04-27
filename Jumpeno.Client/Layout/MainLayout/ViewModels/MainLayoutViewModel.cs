namespace Jumpeno.Client.ViewModels;

public class MainLayoutViewModel(MainLayout layout) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly MainLayout Layout = layout;
    public bool NavigationDisplayed { get; private set; } = true;
    public bool Padding { get; private set; } = true;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void ShowNavigation() {
        NavigationDisplayed = true;
        Padding = true;
        Layout.NotifyState();
    }

    public void HideNavigation(bool keepPadding = true) {
        NavigationDisplayed = false;
        Padding = keepPadding;
        Layout.NotifyState();
    }
}
