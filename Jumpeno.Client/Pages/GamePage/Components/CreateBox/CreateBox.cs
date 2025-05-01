namespace Jumpeno.Client.Components;

public partial class CreateBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Create() {
        await VM.ConnectRequest(new(Game.DEFAULT_CODE, "Tester"), false);
        await PageLoader.Hide(PAGE_LOADER_TASK.GAME_CONNECT);
    }
}
