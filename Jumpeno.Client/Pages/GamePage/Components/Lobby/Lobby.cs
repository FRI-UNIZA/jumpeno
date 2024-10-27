namespace Jumpeno.Client.Components;

public partial class Lobby {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required LobbyViewModel ViewModel { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender) return;
        await ViewModel.OnRender.Invoke();
    }
}
