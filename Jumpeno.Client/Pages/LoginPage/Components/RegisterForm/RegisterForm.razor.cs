namespace Jumpeno.Client.Components;

public partial class RegisterForm {
    // Injections -------------------------------------------------------------------------------------------------------------------------
    [Inject]
    private CookieStorage CookieStorage { get; set; } = null!;
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // Form -------------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<RegisterForm>();
    private readonly InputViewModel<string> VMEmail;
    private readonly InputViewModel<string> VMPlayerName;
    private readonly InputViewModel<string> VMPassword;
    private readonly InputViewModel<string> VMConfirmPassword;
    private ReCAPTCHA ReCAPTCHARef = null!;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string Password = "";
    private bool Success = false;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public RegisterForm() {
        VMEmail = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserRegisterDTO.Email),
            TextMode: InputTextMode.NORMAL,
            Trim: true,
            TextCheck: UserValidator.IsEmail,
            MaxLength: UserValidator.EMAIL_MAX_LENGTH,
            Placeholder: I18N.T("Email"),
            DefaultValue: "",
            OnEnter: new(async e => await Register())
        ));
        VMPlayerName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserRegisterDTO.Name),
            TextMode: InputTextMode.NORMAL,
            Trim: true,
            TextCheck: UserValidator.IsName,
            MaxLength: UserValidator.NAME_MAX_LENGTH,
            Placeholder: I18N.T("Player name"),
            DefaultValue: "",
            OnEnter: new(async e => await Register())
        ));
        VMPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserRegisterDTO.Password),
            TextMode: InputTextMode.NORMAL,
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Placeholder: I18N.T("Password"),
            DefaultValue: "",
            Secret: true,
            OnInput: new(e => {
                Password = e.TextAfter;
                StateHasChanged();
            }),
            OnChange: new(e => {
                if (VMConfirmPassword == null) return;
                if (VMConfirmPassword.Value != e.After) return;
                VMConfirmPassword.Error.Clear();
            }),
            OnEnter: new(async e => await Register())
        ));
        VMConfirmPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: "ConfirmPassword",
            TextMode: InputTextMode.NORMAL,
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Placeholder: I18N.T("Confirm password"),
            DefaultValue: "",
            Secret: true,
            OnEnter: new(async e => await Register())
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Register() {
        if (!await HTTP.Sync(
            async () => {
                if (!CookieStorage.IsCookieAccepted(typeof(Cookies.Security)))
                {
                    await CookieModal.Open(sync: false);
                    Notification.Error(I18N.T("You must accept the security cookie."));
                    return false;
                }
                return true;
            }
        )) return;

        await PageLoader.Show(PageLoaderTask.REGISTRATION);
        await HTTP.Try(async () => {
            // 1) Get CAPTCHA token:
            var captchaToken = await ReCAPTCHARef.GetToken();

            // 2) Create body:
            var body = new UserRegisterDTO(
                Email: VMEmail.Value,
                Name: VMPlayerName.Value,
                Password: VMPassword.Value,
                CAPTCHAToken: captchaToken
            );
                
            // 3) Validation:
            var errors = body.Validate();
            errors.AddRange(UserValidator.ValidateConfirmPassword(VMConfirmPassword.Value, VMPassword.Value, VMConfirmPassword.ID));
            Checker.AssertWith(errors, Exceptions.VALUES);
                
            // 4) Send request:
            var result = await HTTP.Post<MessageDTOR>(API.BASE.USER_REGISTER, body: body);

            // 5) Show result:
            Notification.Success(result.Body.Message);
            Success = true;
            StateHasChanged();
        }, FORM);
        await PageLoader.Hide(PageLoaderTask.REGISTRATION);
    }
}
