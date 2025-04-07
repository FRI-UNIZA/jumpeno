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
            Label: @I18N.T("Email"),
            Placeholder: @I18N.T("Email"),
            DefaultValue: ""
        ));
        VMPassword = new(new InputViewModelTextParams(
            ID: UserValidator.PASSWORD,
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Name: nameof(UserValidator.PASSWORD).ToLower(),
            Label: @I18N.T("Password"),
            Placeholder: @I18N.T("Password"),
            DefaultValue: "",
            Secret: true
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Login() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN_REQUEST);
        await HTTP.Try(async () => {
            // 1) Create data:
            var body = new UserLoginDTO(
                Email: VMEmail.Value,
                Password: VMPassword.Value
            );
            // 2) Validation:
            var errors = body.Validate();
            Checker.CheckValues(errors);
            // 3) Send request:
            var result = await HTTP.Post<MessageDTOR>(API.BASE.USER_LOGIN, body: body);
            // 4) Show result:
            await Navigator.NavigateTo(I18N.Link<HomePage>());
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN_REQUEST);
    }
}
