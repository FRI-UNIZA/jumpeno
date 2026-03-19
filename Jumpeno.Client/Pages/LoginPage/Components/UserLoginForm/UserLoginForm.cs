namespace Jumpeno.Client.Components;

public partial class UserLoginForm {
    // Injections -------------------------------------------------------------------------------------------------------------------------
    [Inject]
    private CookieStorage CookieStorage { get; set; } = null!;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // Form -------------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<UserLoginForm>();
    private readonly InputViewModel<string> VMEmail;
    private readonly InputViewModel<string> VMPassword;
    private ReCAPTCHA ReCAPTCHARef = null!;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public UserLoginForm() {
        VMEmail = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserLoginDTO.Email),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: UserValidator.IsEmail,
            MaxLength: UserValidator.EMAIL_MAX_LENGTH,
            Placeholder: I18N.T("Email"),
            DefaultValue: "",
            OnEnter: new(async e => await Login())
        ));
        VMPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserLoginDTO.Password),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Placeholder: I18N.T("Password"),
            DefaultValue: "",
            Secret: true,
            OnEnter: new(async e => await Login())
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Login() {
        if (!await HTTP.Sync(
            async () => {
                if (!CookieStorage.IsCookieAccepted(typeof(COOKIE.SECURITY)))
                {
                    await CookieModal.Open(sync: false);
                    Notification.Error(I18N.T("You must accept the security cookie."));
                    return false;
                }
                return true;
            }
        )) return;

        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        await HTTP.Try(async () => {
            // 1) Get CAPTCHA token:
            var captchaToken = await ReCAPTCHARef.GetToken();
            // 2) Login:
            await Auth.LogInUser(VMEmail.Value, VMPassword.Value, captchaToken);
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }
}
