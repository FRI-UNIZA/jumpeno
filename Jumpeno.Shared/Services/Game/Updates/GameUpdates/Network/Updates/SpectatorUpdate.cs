namespace Jumpeno.Shared.Models;

public class SpectatorUpdate(ulong id, int round, int spectatorCount) : NetworkUpdate(id, round) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.SPECTATOR_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int SpectatorCount { get; private set; } = spectatorCount;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
