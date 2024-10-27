namespace Jumpeno.Client.Models;

public class MainLayoutController {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private MainLayout Layout;
    public bool NavigationDisplayed { get; private set; }
    public bool ForegroundDisplayed { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public MainLayoutController(MainLayout layout) {
        Layout = layout;
        NavigationDisplayed = true;
        ForegroundDisplayed = false;
    }

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
