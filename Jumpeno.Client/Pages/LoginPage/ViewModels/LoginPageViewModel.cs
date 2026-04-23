namespace Jumpeno.Client.ViewModels;

public class LoginPageViewModel(LoginPage page) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public LoginFormType Form { get; private set; } = LoginFormType.User;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Show(LoginFormType form) {
        Form = form;
        page.Notify();
        ScrollArea.ScrollTo(ScrollAreaId.Page, 0, 0);
    }
}
