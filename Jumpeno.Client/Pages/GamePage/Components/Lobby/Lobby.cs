namespace Jumpeno.Client.Components;

public partial class Lobby : IDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel VM { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputePlayerLineClass(bool current) {
        var c = new CSSClass("player-line");
        if (current) c.Set("current");
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
    }
}
