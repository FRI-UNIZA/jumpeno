namespace Jumpeno.Shared.Models;

public abstract class NetworkUpdate(ulong id, int round) : GameUpdate {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public abstract string HUB_ACTION { get; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public ulong ID { get; private set; } = id;
    public int Round { get; private set; } = round;
}
