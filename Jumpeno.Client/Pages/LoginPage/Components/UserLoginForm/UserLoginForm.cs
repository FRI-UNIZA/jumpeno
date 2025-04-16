namespace Jumpeno.Client.Components;

public partial class UserLoginForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMEmail;
    private readonly InputViewModel<string> VMPassword;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public UserLoginForm() {
        VMEmail = new(new InputViewModelTextParams(
            ID: UserValidator.EMAIL,
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsEmail,
            MaxLength: UserValidator.EMAIL_MAX_LENGTH,
            Name: nameof(UserValidator.EMAIL).ToLower(),
            Label: I18N.T("Email"),
            Placeholder: I18N.T("Email"),
            DefaultValue: ""
        ));
        VMPassword = new(new InputViewModelTextParams(
            ID: UserValidator.PASSWORD,
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Name: nameof(UserValidator.PASSWORD).ToLower(),
            Label: I18N.T("Password"),
            Placeholder: I18N.T("Password"),
            DefaultValue: "",
            Secret: true
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Login() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        await HTTP.Try(async () => {
            // 1) Login:
            await Auth.LogInUser(VMEmail.Value, VMPassword.Value);
            // 2) Show result:
            await Navigator.NavigateTo(AuthPage.LINK_FALLBACK);
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }

    private async Task Refresh() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        // await HTTP.Try(Auth.Refresh);
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }

    private async Task Profile() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        await HTTP.Try(async () => {
            // 3) Send request:
            var result = await HTTP.Get<UserProfileDTOR>(API.BASE.USER_PROFILE);
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }

    private async Task LockTest() {
        await Window.Lock(async () => {
            Console.WriteLine("Vivat start!!!");
            await Task.Delay(5000);
            Console.WriteLine("Vivat!!!");
        }, WINDOW_LOCK.AUTHENTICATION);
        Console.WriteLine("Vivat completed!!!!!!");
    }

    private async Task LockTest2() {
        await Window.Lock(async () => {
            Console.WriteLine("BBB start!!!");
            await Task.Delay(5000);
            Console.WriteLine("BBB!!!");
        });
        Console.WriteLine("BBB completed!!!!!!");
    }

    private async Task Delete() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        await HTTP.Try(async () => {
            // 1) Login:
            await Auth.LogOut();
            // 2) Show result:
            await Navigator.NavigateTo(AuthPage.LINK_FALLBACK);
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }
}
