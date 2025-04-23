namespace Jumpeno.Client.ViewModels;

public class LoginPageViewModel(LoginPage page) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public LOGIN_FORM Form { get; private set; } = LOGIN_FORM.USER;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public void Show(LOGIN_FORM form) {
        Form = form;
        page.Notify();
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }
}
