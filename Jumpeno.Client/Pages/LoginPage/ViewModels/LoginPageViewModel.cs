namespace Jumpeno.Client.ViewModels;

public class LoginPageViewModel(LoginPage page) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public LoginFormType Form { get; private set; } = LoginFormType.USER;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Show(LoginFormType form) {
        Form = form;
        page.Notify();
        ScrollArea.ScrollTo(ScrollAreaId.PAGE, 0, 0);
    }
}
