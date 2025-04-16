namespace Jumpeno.Client.Components;

public partial class AdminLoginForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------    
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMEmail;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool Verified = false;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public AdminLoginForm() {
        VMEmail = new(new InputViewModelTextParams(
            ID: AdminValidator.EMAIL,
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsEmail,
            MaxLength: AdminValidator.EMAIL_MAX_LENGTH,
            Name: nameof(AdminValidator.EMAIL).ToLower(),
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
            var body = new AdminLoginDTO(
                Email: VMEmail.Value
            );
            // 2) Validation:
            body.Check();
            // 3) Send request:
            var response = await HTTP.Post<MessageDTOR>(API.BASE.ADMIN_LOGIN, body: body);
            // 4) Show result:
            Notification.Success(response.Body.Message);
            Verified = true;
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }
}
