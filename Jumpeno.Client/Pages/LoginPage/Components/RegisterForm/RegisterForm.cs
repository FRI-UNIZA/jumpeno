namespace Jumpeno.Client.Components;

public partial class RegisterForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMEmail;
    private readonly InputViewModel<string> VMPlayerName;
    private readonly InputViewModel<string> VMPassword;
    private readonly InputViewModel<string> VMConfirmPassword;
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool Success = false;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public RegisterForm() {
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
        VMPlayerName = new(new InputViewModelTextParams(
            ID: UserValidator.NAME,
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsAlphaNum,
            MaxLength: UserValidator.NAME_MAX_LENGTH,
            Name: nameof(UserValidator.NAME).ToLower(),
            Label: I18N.T("Player name"),
            Placeholder: I18N.T("Player name"),
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
            Secret: true,
            OnChange: new(value => {
                if (VMConfirmPassword == null) return;
                if (VMConfirmPassword.Value != value) return;
                VMConfirmPassword.Error.ClearError();
            })
        ));
        VMConfirmPassword = new(new InputViewModelTextParams(
            ID: UserValidator.PASSWORD_CONFIRM,
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Name: nameof(UserValidator.PASSWORD_CONFIRM).ToLower(),
            Label: I18N.T("Confirm password"),
            Placeholder: I18N.T("Confirm password"),
            DefaultValue: "",
            Secret: true
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Register() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        await HTTP.Try(async () => {
            // 1) Create body:
            var body = new UserRegisterDTO(
                Email: VMEmail.Value,
                Name: VMPlayerName.Value,
                Password: VMPassword.Value
            );
            // 2) Validation:
            var errors = body.Validate();
            if (VMConfirmPassword.Value.Trim() == "") {
                errors.Add(new Error(UserValidator.PASSWORD_CONFIRM, Checker.FIELD_EMPTY));
            }
            if (VMPassword.Value != VMConfirmPassword.Value) {
                errors.Add(new Error(UserValidator.PASSWORD_CONFIRM, Checker.FIELD_NOT_MATCH));
            }
            Checker.CheckValues(errors);
            // 3) Send request:
            var result = await HTTP.Post<MessageDTOR>(API.BASE.USER_REGISTER, body: body);
            // 4) Show result:
            Success = true;
            Notification.Success(result.Body.Message);
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }
}
