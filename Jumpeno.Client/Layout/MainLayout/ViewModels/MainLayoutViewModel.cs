namespace Jumpeno.Client.ViewModels;

public class MainLayoutViewModel(MainLayout layout)
{
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly MainLayout Layout = layout;
    public bool NavigationDisplayed { get; private set; } = true;
    public bool ForegroundDisplayed { get; private set; } = false;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void ShowNavigation() {
        NavigationDisplayed = true;
        Layout.Notify();
    }

    public void HideNavigation() {
        NavigationDisplayed = false;
        Layout.Notify();
    }

    public void ShowForeground() {
        ForegroundDisplayed = true;
        Layout.Notify();
    }

    public void HideForeground() {
        ForegroundDisplayed = false;
        Layout.Notify();
    }
}
