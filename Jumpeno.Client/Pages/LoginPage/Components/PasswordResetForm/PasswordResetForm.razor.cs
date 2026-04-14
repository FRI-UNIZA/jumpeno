namespace Jumpeno.Client.Components;

public partial class PasswordResetForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------    
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<PasswordResetForm>();
    private readonly InputViewModel<string> VMEmail;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public PasswordResetForm() {
        VMEmail = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserPasswordResetRequestDTO.Email),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: UserValidator.IsEmail,
            MaxLength: UserValidator.EMAIL_MAX_LENGTH,
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
            var body = new UserPasswordResetRequestDTO(
                Email: VMEmail.Value
            );
            // 2) Validation:
            body.Assert();
            // 3) Send request:
            var response = await HTTP.Post<MessageDTOR>(API.BASE.USER_PASSWORD_RESET_REQUEST, body: body);
            // 4) Show result:
            Notification.Success(response.Body.Message);
            VM.Show(LOGIN_FORM.USER);
            ActionHandler.PopFocus();
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN);
    }
}
