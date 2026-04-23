namespace Jumpeno.Client.Models;

public class KeyUpdate(ulong id, int round, byte playerID, LinkedList<Control> controls) : NetworkUpdate(id, round) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HubAction => GameHubs.KeyUpdate;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte PlayerID { get; private set; } = playerID;
    public LinkedList<Control> Controls { get; private set; } = controls;
    
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
