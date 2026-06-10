namespace Jumpeno.Client.Components;

public partial class AccountTab : IProfileTab
{
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly ICollection<Skin> Skins = Enum.GetValues<Skin>();

    // Forms ------------------------------------------------------------------------------------------------------------------------------
    private readonly string FORM = Form.Of<AccountTab>();
    private readonly InputViewModel<string> VMPlayerName;
    private readonly InputViewModel<string> VMEmail;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter] public required Modal ModalRef { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private ConfirmModal ConfirmModalRef { get ; set; } = null!;
    private AntDesign.Popover SkinPopover = new();

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set("account-tab", Base);

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
            MaxLength: UserValidator.NameMaxLength,
            OnEnter: new(async e => await UpdateUserProfileInfo())
        ));
        VMEmail = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(UserUpdateDTO.NewEmail),
            Trim: true,
            Placeholder: I18N.T("Email"),
            DefaultValue: Auth.User.Email ?? "",
            TextCheck: UserValidator.IsEmail,
            MaxLength: UserValidator.EmailMaxLength
        ));
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private async Task SendActivationLink()
    {
        await PageLoader.Show(PageLoaderTask.Activation);
        await HTTP.Try(async () => {
            var result = await HTTP.Post<MessageDTOR>(API.Base.UserSendActivation);
            var body = result.Body.Assert();
            Notification.Success(body.Message);
        }, FORM);
        await PageLoader.Hide(PageLoaderTask.Activation);
    }

    private async Task UpdateUserProfileInfo() 
    {
        await PageLoader.Show(PageLoaderTask.UserUpdate);
        await HTTP.Try(async () => {
            var model = new UserUpdateDTO(NewName: VMPlayerName.Value);
            var result = await HTTP.Patch<MessageDTOR>(API.Base.UserUpdate, body: model);
            var body = result.Body.Assert();
            
            await Auth.LoadProfile();
            await ResetForm();
            Notification.Success(body.Message);
        }, FORM);
        await PageLoader.Hide(PageLoaderTask.UserUpdate);
    }

    private async Task DeleteAccount() 
    {
        await ConfirmModalRef.Open(async () => {
            await PageLoader.Show(PageLoaderTask.DeleteAccount);
            await Auth.RequestFreeze();
            await HTTP.Try(async () => {
                var result = await HTTP.Delete<MessageDTOR>(API.Base.UserDelete);
                var body = result.Body.Assert();
                await Auth.LogOut();
                await ModalRef.Close();
                Notification.Success(body.Message);
            }, FORM);
            await Auth.ResolveFreeze();
            await PageLoader.Hide(PageLoaderTask.DeleteAccount);
        });
    }

    private async Task ChangeSkin(Skin skin)
    {
        await PageLoader.Show(PageLoaderTask.UserUpdate);
        await SkinPopover.Close();
        if (Auth.User.Skin != skin)
        {
            await HTTP.Try(async () => {
                var model = new UserUpdateDTO(NewSkin: skin);

                var result = await HTTP.Patch<MessageDTOR>(API.Base.UserUpdate, body: model);
                var body = result.Body.Assert();

                await Auth.LoadProfile();
                await ResetForm();
                Notification.Success(result.Body.Message);
            }, FORM);
        }
        await PageLoader.Hide(PageLoaderTask.UserUpdate);
    }

    public Task ResetForm()
    {
        VMPlayerName.SetValue(Auth.User.Name ?? "");
        VMEmail.SetValue(Auth.User.Email ?? "");
        return Task.CompletedTask;
    }
}
