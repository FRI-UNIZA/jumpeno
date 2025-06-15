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
            await VM.ConnectRequest(new(Game.DEFAULT_CODE, Auth.User.Name), false);
        } else {
            Notification.Error(I18N.T("This functionality is not implemented yet."));
        }
    }
}
