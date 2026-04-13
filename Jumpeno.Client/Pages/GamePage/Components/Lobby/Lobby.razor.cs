namespace Jumpeno.Client.Components;

public partial class Lobby {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required GameViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private GameConfirmModal DeleteConfirmModalRef = null!;
    private GameModal QRCodeModalRef = null!;
    private GameConfirmModal PlayerKickConfirmModalRef = null!;
    private Player? PlayerToKick = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() {
        return base.ComputeClass().Set("lobby", Base).Set(VM.CSSClass())
        .Set("player-settings-open", PlayerSettingsOpen);
    }

    private CSSClass PlayerLineClass(Player player) {
        return new CSSClass("player-line")
        .Set("current", VM.Player != null && VM.Player.Equals(player))
        .Set(player.IsConnected ? "connected" : "disconnected")
        .Set("ready", player.IsReady(VM.Game));
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnComponentAfterRenderAsync(bool firstRender) { if (firstRender) await VM.StartUpdating(); }

    protected override void OnComponentDispose() => VM.StopUpdating();

    // Players ----------------------------------------------------------------------------------------------------------------------------
    private bool PlayerSettingsOpen = false;

    private void TogglePlayerSettings() {
        PlayerSettingsOpen = !PlayerSettingsOpen;
        Notify();
    }
}
