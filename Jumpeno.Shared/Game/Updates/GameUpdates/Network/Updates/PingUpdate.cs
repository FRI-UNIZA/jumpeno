namespace Jumpeno.Shared.Models;

public class PingUpdate(ulong id, DateTime createdAt) : NetworkUpdate(id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.PING_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public DateTime CreatedAt { get; private set; } = createdAt;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
