namespace Jumpeno.Client.Components;

public partial class AvatarTab : IProfileTab
{
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly ICollection<SKIN> Skins = Enum.GetValues<SKIN>();

    // Forms ------------------------------------------------------------------------------------------------------------------------------
    private readonly string FORM = Form.Of<AvatarTab>();
    private SKIN SelectedSkin = Auth.User.Skin;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set("avatar-tab", Base);
    private string ComputeAvatarOptionClass(SKIN skin) => new CSSClass("avatar-option").Set("selected", SelectedSkin == skin);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void SelectSkin(SKIN skin) 
    {
        SelectedSkin = skin;
        StateHasChanged();
    }

    private async Task ChangeSkin() 
    {
        await PageLoader.Show(PAGE_LOADER_TASK.USER_UPDATE);
        await HTTP.Try(async () => {
            var model = new UserUpdateDTO(NewSkin: SelectedSkin);

            var result = await HTTP.Patch<MessageDTOR>(API.BASE.USER_UPDATE, body: model);
            var body = result.Body.Assert();

            await Auth.LoadProfile();
            await ResetForm();
            Notification.Success(result.Body.Message);
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.USER_UPDATE);
    }

    public Task ResetForm()
    {
        SelectSkin(Auth.User.Skin);
        return Task.CompletedTask;
    }
}
