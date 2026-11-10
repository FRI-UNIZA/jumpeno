namespace Jumpeno.Client.Models;

public abstract class NetworkUpdate(ulong id, int round) : GameUpdate {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public abstract string HubAction { get; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public ulong ID { get; private set; } = id;
    public int Round { get; private set; } = round;

    /// <summary>ConnectionIDs for response.</summary>
    public LinkedList<string>? ResponseIDs { get; set; } = null;
}
