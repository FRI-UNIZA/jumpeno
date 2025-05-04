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
    public readonly string FORM = Form.Of<RegisterForm>();
    private bool Success = false;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public RegisterForm() {
        VMEmail = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserRegisterDTO.Email),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsEmail,
            MaxLength: UserValidator.EMAIL_MAX_LENGTH,
            Name: nameof(UserRegisterDTO.Email),
            Label: I18N.T("Email"),
            Placeholder: I18N.T("Email"),
            DefaultValue: "",
            OnEnter: new(Register)
        ));
        VMPlayerName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserRegisterDTO.Name),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsAlphaNum,
            MaxLength: UserValidator.NAME_MAX_LENGTH,
            Name: nameof(UserRegisterDTO.Name),
            Label: I18N.T("Player name"),
            Placeholder: I18N.T("Player name"),
            DefaultValue: "",
            OnEnter: new(Register)
        ));
        VMPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserRegisterDTO.Password),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            TextCheck: Checker.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Name: nameof(UserRegisterDTO.Password),
            Label: I18N.T("Password"),
            Placeholder: I18N.T("Password"),
            DefaultValue: "",
            Secret: true,
            OnChange: new(value => {
                if (VMConfirmPassword == null) return;
                if (VMConfirmPassword.Value != value) return;
                VMConfirmPassword.Error.ClearError();
            }),
            OnEnter: new(Register)
        ));
        VMConfirmPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: "ConfirmPassword",
            TextMode: INPUT_TEXT_MODE.NORMAL,
            TextCheck: Checker.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Name: "ConfirmPassword",
            Label: I18N.T("Confirm password"),
            Placeholder: I18N.T("Confirm password"),
            DefaultValue: "",
            Secret: true,
            OnEnter: new(Register)
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Register() {
        await PageLoader.Show(PAGE_LOADER_TASK.REGISTRATION);
        await HTTP.Try(async () => {
            // 1) Create body:
            var body = new UserRegisterDTO(
                Email: VMEmail.Value,
                Name: VMPlayerName.Value,
                Password: VMPassword.Value
            );
            // 2) Validation:
            var errors = body.Validate();
            errors.AddRange(UserValidator.ValidateConfirmPassword(VMConfirmPassword.Value, VMPassword.Value, VMConfirmPassword.ID));
            Checker.AssertWith(errors, EXCEPTION.VALUES);
            // 3) Send request:
            var result = await HTTP.Post<MessageDTOR>(API.BASE.USER_REGISTER, body: body);
            // 4) Show result:
            Notification.Success(result.Body.Message);
            Success = true;
            StateHasChanged();
            ActionHandler.PopFocus();
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.REGISTRATION);
    }
}
