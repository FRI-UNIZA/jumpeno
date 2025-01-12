namespace Jumpeno.Shared.Models;

public class PlayerUpdate(ulong id, int round, Player player) : NetworkUpdate(id, round) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.PLAYER_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Player Player { get; private set; } = player;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
