namespace Jumpeno.Client.Models;

public class PlayerUpdate(ulong id, int round, bool hostConnected, Player player, bool anonymize) : NetworkUpdate(id, round) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.PLAYER_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public bool HostConnected { get; private set; } = hostConnected;
    public Player Player { get; private set; } = player;
    public bool Anonymize { get; private set; } = anonymize;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
