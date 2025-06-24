namespace Jumpeno.Client.Components;

public partial class UserLoginForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<UserLoginForm>();
    private readonly InputViewModel<string> VMEmail;
    private readonly InputViewModel<string> VMPassword;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public UserLoginForm() {
        VMEmail = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserLoginDTO.Email),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsEmail,
            MaxLength: UserValidator.EMAIL_MAX_LENGTH,
            Placeholder: I18N.T("Email"),
            DefaultValue: "",
            OnEnter: new(Login)
        ));
        VMPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserLoginDTO.Password),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            TextCheck: Checker.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Placeholder: I18N.T("Password"),
            DefaultValue: "",
            Secret: true,
            OnEnter: new(Login)
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Login() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        await HTTP.Try(async () => {
            await Auth.LogInUser(VMEmail.Value, VMPassword.Value);
            ActionHandler.PopFocus();
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }
}
