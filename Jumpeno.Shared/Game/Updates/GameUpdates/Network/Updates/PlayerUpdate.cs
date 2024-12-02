namespace Jumpeno.Shared.Models;

public class PlayerUpdate(ulong id, Player player, PLAYER_ACTION action) : NetworkUpdate(id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.PLAYER_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Player Player { get; private set; } = player;
    public PLAYER_ACTION Action { get; private set; } = action;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
