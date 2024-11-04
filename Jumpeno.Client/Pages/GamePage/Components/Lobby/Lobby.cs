namespace Jumpeno.Client.Components;

public partial class Lobby {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel ViewModel { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputePlayerLineClass(bool current) {
        var c = new CSSClass("player-line");
        if (current) c.Set("current");
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender) return;
        await ViewModel.OnRender.Invoke();
    }
}
