namespace Jumpeno.Client.Components;

public partial class AdminLoginForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------    
    [Parameter]
    public required LoginPageViewModel VM { get; set; }
    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool Verified = false;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<AdminLoginForm>();
    private readonly InputViewModel<string> VMEmail;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public AdminLoginForm() {
        VMEmail = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(AdminLoginDTO.Email),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: AdminValidator.IsEmail,
            MaxLength: AdminValidator.EMAIL_MAX_LENGTH,
            Placeholder: I18N.T("Email"),
            DefaultValue: "",
            OnEnter: new(async e => await Send())
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Send() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN);
        await HTTP.Try(async () => {
            // 1) Create body:
            var body = new AdminLoginDTO(
                Email: VMEmail.Value
            );
            // 2) Validation:
            body.Assert();
            // 3) Send request:
            var response = await HTTP.Post<MessageDTOR>(API.BASE.ADMIN_LOGIN, body: body);
            // 4) Show result:
            Notification.Success(response.Body.Message);
            Verified = true;
            StateHasChanged();
            ActionHandler.PopFocus();
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }
}
