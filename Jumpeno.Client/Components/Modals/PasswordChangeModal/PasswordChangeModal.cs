namespace Jumpeno.Client.Components;

public partial class PasswordChangeModal
{
    // ViewModels ---------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Forms -----------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<PasswordChangeModal>();
    private readonly InputViewModel<string> VMPassword;
    private readonly InputViewModel<string> VMConfirmPassword;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public PasswordChangeModal()
    {
        VMPassword = new(new InputViewModelTextParams(
           Form: FORM,
           ID: nameof(UserPasswordChangeDTO.NewPassword),
           TextMode: INPUT_TEXT_MODE.NORMAL,
           TextCheck: UserValidator.IsPassword,
           MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
           Placeholder: I18N.T("Password"),
           DefaultValue: "",
           Secret: true,
           OnChange: new(e => {
               if (VMConfirmPassword == null) return;
               if (VMConfirmPassword.Value != e.After) return;
               VMConfirmPassword.Error.Clear();
           }),
           OnEnter: new(async e => await ChangePassword())
        ));
        VMConfirmPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: "ConfirmPasssword",
            TextMode: INPUT_TEXT_MODE.NORMAL,
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            Placeholder: I18N.T("Confirm password"),
            DefaultValue: "",
            Secret: true,
            OnEnter: new(async e => await ChangePassword())
        ));
    }

    // Methods --------------------------------------------------------------------------------------------------------------------------
    public async Task Open()
    {
        await ModalRef.OpenLoading();
        var success = await HTTP.Try(Auth.LoadProfile);
        if (success) await ModalRef.FinishLoading();
        else await ModalRef.CloseLoading();
    }

    public async Task ChangePassword() 
    {
        await PageLoader.Show(PAGE_LOADER_TASK.PASSWORD_CHANGE);

        await HTTP.Try(async () => {
            // 1) Create body:
            var body = new UserPasswordChangeDTO(
                NewPassword: VMPassword.Value
            );
            // 2) Validation:
            var errors = new List<Error>();
            errors.AddRange(body.Validate());
            errors.AddRange(UserValidator.ValidateConfirmPassword(VMConfirmPassword.Value, VMPassword.Value, VMConfirmPassword.ID));
            Checker.AssertWith(errors, EXCEPTION.VALUES);
            // 3) Send request:
            var response = await HTTP.Patch<MessageDTOR>(API.BASE.USER_PASSWORD_CHANGE, body: body);
            // 4) Show result:
            await Auth.LogOut();
            await Modal.CloseAll();
            Notification.Success(response.Body.Message);
        }, FORM);

        await PageLoader.Hide(PAGE_LOADER_TASK.PASSWORD_CHANGE);
    }

    public Task ResetForm()
    {
        VMPassword.SetValue("");
        VMConfirmPassword.SetValue("");
        return Task.CompletedTask;
    }
}
