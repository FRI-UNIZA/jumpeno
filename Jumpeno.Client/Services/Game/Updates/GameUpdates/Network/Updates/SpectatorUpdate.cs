namespace Jumpeno.Client.Models;

public class SpectatorUpdate(ulong id, int round, bool hostConnected, int spectatorCount) : NetworkUpdate(id, round) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.SPECTATOR_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool HostConnected { get; private set; } = hostConnected;
    public int SpectatorCount { get; private set; } = spectatorCount;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
