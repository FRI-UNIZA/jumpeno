namespace Jumpeno.Shared.Models;

public class WatchUpdate(ulong id, int spectatorCount) : NetworkUpdate(id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.WATCH_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int SpectatorCount { get; private set; } = spectatorCount;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
