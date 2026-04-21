namespace Jumpeno.Client.Components;

public partial class PasswordChangeTab : IProfileTab
{
    // Forms ------------------------------------------------------------------------------------------------------------------------------
    private readonly string FORM = Form.Of<PasswordChangeTab>();

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMOldPassword;
    private readonly InputViewModel<string> VMNewPassword;
    private readonly InputViewModel<string> VMNewPasswordConfirm;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string Password = "";

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set("password-change-tab", Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public PasswordChangeTab()
    {
        VMOldPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserPasswordChangeDTO.OldPassword),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Secret: true,
            Trim: true,
            Placeholder: "••••••••",
            DefaultValue: "",
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            OnEnter: new(async e => await ChangePassword())
        ));
        VMNewPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserPasswordChangeDTO.NewPassword),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Secret: true,
            Trim: true,
            Placeholder: "••••••••",
            DefaultValue: "",
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            OnInput: new(e => {
                Password = e.TextAfter;
                StateHasChanged();
            }),
            OnEnter: new(async e => await ChangePassword())
        ));
        VMNewPasswordConfirm = new(new InputViewModelTextParams(
            Form: FORM,
            ID: "ConfirmPassword",
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Secret: true,
            Trim: true,
            Placeholder: "••••••••",
            DefaultValue: "",
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PASSWORD_MAX_LENGTH,
            OnEnter: new(async e => await ChangePassword())
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task ChangePassword()
    {
        await PageLoader.Show(PAGE_LOADER_TASK.PASSWORD_CHANGE);

        await HTTP.Try(async () => {
            // 1) Create body:
            var body = new UserPasswordChangeDTO(
                NewPassword: VMNewPassword.Value,
                OldPassword: VMOldPassword.Value
            );
            // 2) Validation:
            var errors = new List<Error>();

            errors.AddRange(body.Validate());
            errors.AddRange(UserValidator.ValidateConfirmPassword(VMNewPasswordConfirm.Value, VMNewPassword.Value, VMNewPasswordConfirm.ID));
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
        VMOldPassword.SetValue("");
        VMNewPassword.SetValue("");
        VMNewPasswordConfirm.SetValue("");
        return Task.CompletedTask;
    }
}
