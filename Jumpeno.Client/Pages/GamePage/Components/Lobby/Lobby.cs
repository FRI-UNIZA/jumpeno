namespace Jumpeno.Client.Components;

public partial class Lobby {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private GameConfirmModal DeleteConfirmModalRef = null!;
    private GameModal QRCodeModalRef = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set("lobby", Base).Set(VM.CSSClass());

    private CSSClass PlayerLineClass(Player player) {
        return new CSSClass("player-line")
        .Set("current", VM.Player != null && VM.Player.Equals(player))
        .Set(player.IsConnected ? "connected" : "disconnected");
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnComponentAfterRenderAsync(bool firstRender) { if (firstRender) await VM.StartUpdating(); }

    protected override void OnComponentDispose() => VM.StopUpdating();
}
