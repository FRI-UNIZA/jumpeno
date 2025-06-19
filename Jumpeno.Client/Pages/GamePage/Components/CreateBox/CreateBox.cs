namespace Jumpeno.Client.Components;

public partial class CreateBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Create() {
        var implemented = false;
        // TODO: Implement the logic to create a game
        if (implemented) {
            await VM.PlayRequest(new(Game.DEFAULT_CODE, Auth.User.Name));
        } else {
            Notification.Error(I18N.T("This functionality is not implemented yet."));
        }
    }
}
