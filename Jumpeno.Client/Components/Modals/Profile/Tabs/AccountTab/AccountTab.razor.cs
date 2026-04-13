namespace Jumpeno.Client.Components;

public partial class AccountTab : IProfileTab
{
    // Forms ------------------------------------------------------------------------------------------------------------------------------
    private readonly string FORM = Form.Of<AccountTab>();
    private readonly InputViewModel<string> VMPlayerName;
    private readonly InputViewModel<string> VMEmail;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter] public required Modal ModalRef { get; set; }
    [Parameter] public required PasswordChangeModal PasswordChangeModalRef { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private ConfirmModal ConfirmModalRef { get ; set; } = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set("account-tab", Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public AccountTab()
    {
        VMPlayerName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserUpdateDTO.NewName),
            Trim: true,
            Placeholder: I18N.T("Player name"),
            DefaultValue: Auth.User.Name ?? "",
            TextCheck: UserValidator.IsName,
            MaxLength: UserValidator.NAME_MAX_LENGTH
        ));
        VMEmail = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserUpdateDTO.NewEmail),
            Trim: true,
            Placeholder: I18N.T("Email"),
            DefaultValue: Auth.User.Email ?? "",
            TextCheck: UserValidator.IsEmail,
            MaxLength: UserValidator.EMAIL_MAX_LENGTH
        ));
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private async Task SendActivationLink()
    {
        await PageLoader.Show(PAGE_LOADER_TASK.ACTIVATION);
        await HTTP.Try(async () => {
            var result = await HTTP.Post<MessageDTOR>(API.BASE.USER_SEND_ACTIVATION);
            var body = result.Body.Assert();
            Notification.Success(body.Message);
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.ACTIVATION);
    }

    private async Task UpdateUserProfileInfo() 
    {
        await PageLoader.Show(PAGE_LOADER_TASK.USER_UPDATE);
        await HTTP.Try(async () => {
            var model = new UserUpdateDTO(NewName: VMPlayerName.Value, NewEmail: VMEmail.Value);
            var result = await HTTP.Patch<MessageDTOR>(API.BASE.USER_UPDATE, body: model);
            var body = result.Body.Assert();
            
            await Auth.LoadProfile();
            await ResetForm();
            Notification.Success(body.Message);
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.USER_UPDATE);
    }

    private async Task DeleteAccount() 
    {
        await ConfirmModalRef.Open(async () => {
            await PageLoader.Show(PAGE_LOADER_TASK.DELETE_ACCOUNT);
            await Auth.RequestFreeze();
            await HTTP.Try(async () => {
                var result = await HTTP.Delete<MessageDTOR>(API.BASE.USER_DELETE);
                var body = result.Body.Assert();
                await Auth.LogOut();
                await ModalRef.Close();
                Notification.Success(body.Message);
            }, FORM);
            await Auth.ResolveFreeze();
            await PageLoader.Hide(PAGE_LOADER_TASK.DELETE_ACCOUNT);
        });
    }

    private async Task ChangePassword()
    {
        await PasswordChangeModalRef.Open();
    }

    public Task ResetForm() 
    {
        VMPlayerName.SetValue(Auth.User.Name ?? "");
        VMEmail.SetValue(Auth.User.Email ?? "");
        return Task.CompletedTask;
    }
}
