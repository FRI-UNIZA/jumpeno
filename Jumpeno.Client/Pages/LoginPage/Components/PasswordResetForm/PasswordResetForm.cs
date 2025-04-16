namespace Jumpeno.Client.Components;

public partial class PasswordResetForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------    
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMEmail;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public PasswordResetForm() {
        VMEmail = new(new InputViewModelTextParams(
            ID: UserValidator.EMAIL,
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsEmail,
            MaxLength: UserValidator.EMAIL_MAX_LENGTH,
            Name: nameof(UserValidator.EMAIL).ToLower(),
            Label: I18N.T("Email address verification"),
            Placeholder: I18N.T("Email"),
            DefaultValue: ""
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Login() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        await HTTP.Try(async () => {
            // 1) Create body:
            var body = new UserPasswordResetRequestDTO(
                Email: VMEmail.Value
            );
            // 2) Validation:
            body.Check();
            // 3) Send request:
            var response = await HTTP.Post<MessageDTOR>(API.BASE.USER_PASSWORD_RESET_REQUEST, body: body);
            // 4) Show result:
            Notification.Success(response.Body.Message);
            VM.Show(LOGIN_FORM.USER);
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }
}
