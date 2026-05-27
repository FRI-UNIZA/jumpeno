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
    public override CssClass ComputeClass() => base.ComputeClass().Set("password-change-tab", Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public PasswordChangeTab()
    {
        VMOldPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserPasswordChangeDTO.OldPassword),
            TextMode: InputTextMode.Normal,
            Secret: true,
            Trim: true,
            Placeholder: "••••••••",
            DefaultValue: "",
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PasswordMaxLength,
            OnEnter: new(async e => await ChangePassword())
        ));
        VMNewPassword = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserPasswordChangeDTO.NewPassword),
            TextMode: InputTextMode.Normal,
            Secret: true,
            Trim: true,
            Placeholder: "••••••••",
            DefaultValue: "",
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PasswordMaxLength,
            OnInput: new(e => {
                Password = e.TextAfter;
                StateHasChanged();
            }),
            OnEnter: new(async e => await ChangePassword())
        ));
        VMNewPasswordConfirm = new(new InputViewModelTextParams(
            Form: FORM,
            ID: "ConfirmPassword",
            TextMode: InputTextMode.Normal,
            Secret: true,
            Trim: true,
            Placeholder: "••••••••",
            DefaultValue: "",
            TextCheck: UserValidator.IsPassword,
            MaxLength: UserValidator.PasswordMaxLength,
            OnEnter: new(async e => await ChangePassword())
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task ChangePassword()
    {
        await PageLoader.Show(PageLoaderTask.PasswordChange);

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
            Checker.AssertWith(errors, Exceptions.Values);
            // 3) Send request:
            var response = await HTTP.Patch<MessageDTOR>(API.Base.UserPasswordChange, body: body);
            // 4) Show result:
            await Auth.LogOut();
            await Modal.CloseAll();
            Notification.Success(response.Body.Message);
        }, FORM);

        await PageLoader.Hide(PageLoaderTask.PasswordChange);
    }

    public Task ResetForm()
    {
        VMOldPassword.SetValue("");
        VMNewPassword.SetValue("");
        VMNewPasswordConfirm.SetValue("");
        return Task.CompletedTask;
    }
}
