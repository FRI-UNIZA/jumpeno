namespace Jumpeno.Client.ViewModels;

public class AppLayoutVM {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool NavigationDisplayed { get; private set; } = true;
    public bool Padding { get; private set; } = true;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void ShowNavigation() {
        NavigationDisplayed = true;
        Padding = true;
        AppLayout.Notify(NOTIFY.STATE);
    }

    public void HideNavigation(bool keepPadding = true) {
        NavigationDisplayed = false;
        Padding = keepPadding;
        AppLayout.Notify(NOTIFY.STATE);
    }
}
