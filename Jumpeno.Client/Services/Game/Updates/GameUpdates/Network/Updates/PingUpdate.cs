namespace Jumpeno.Client.Models;

public class PingUpdate(ulong id, int round, DateTime createdAt) : NetworkUpdate(id, round) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.PING_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime? ReturnedAt { get; private set; } = null;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void SetReturn() => ReturnedAt = DateTime.UtcNow;
    public override string ToString() => Format.JSON_PRETTY(this);
}
