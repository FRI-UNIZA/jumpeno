namespace Jumpeno.Shared.Models;

public abstract class NetworkUpdate(ulong id) : GameUpdate {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public abstract string HUB_ACTION { get; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public ulong ID { get; private set; } = id;
}
