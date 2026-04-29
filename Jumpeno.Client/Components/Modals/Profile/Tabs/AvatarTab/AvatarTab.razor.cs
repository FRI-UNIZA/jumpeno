namespace Jumpeno.Client.Components;

public partial class AvatarTab : IProfileTab
{
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly ICollection<Skin> Skins = Enum.GetValues<Skin>();

    // Forms ------------------------------------------------------------------------------------------------------------------------------
    private readonly string form = Form.Of<AvatarTab>();
    private Skin SelectedSkin = Auth.User.Skin;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set("avatar-tab", Base);
    private string ComputeAvatarOptionClass(Skin skin) => new CssClass("avatar-option").Set("selected", SelectedSkin == skin);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void SelectSkin(Skin skin) 
    {
        SelectedSkin = skin;
        StateHasChanged();
    }

    private async Task ChangeSkin() 
    {
        await PageLoader.Show(PageLoaderTask.UserUpdate);
        await HTTP.Try(async () => {
            var model = new UserUpdateDTO(NewSkin: SelectedSkin);

            var result = await HTTP.Patch<MessageDTOR>(API.Base.UserUpdate, body: model);
            var body = result.Body.Assert();

            await Auth.LoadProfile();
            await ResetForm();
            Notification.Success(result.Body.Message);
        }, form);
        await PageLoader.Hide(PageLoaderTask.UserUpdate);
    }

    public Task ResetForm()
    {
        SelectSkin(Auth.User.Skin);
        return Task.CompletedTask;
    }
}
