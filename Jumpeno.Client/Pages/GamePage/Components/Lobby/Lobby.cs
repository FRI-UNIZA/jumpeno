namespace Jumpeno.Client.Components;

public partial class Lobby : IDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel VM { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputePlayerLineClass(Player player) {
        var c = new CSSClass("player-line");
        if (VM.Player != null && VM.Player.Equals(player)) c.Set("current");
        c.Set(player.IsConnected ? "connected" : "disconnected");
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender) return;
        await VM.InitOnRender();
        await VM.StartUpdating();
    }

    public void Dispose() {
        VM.StopUpdating();
        GC.SuppressFinalize(this);
    }
}
