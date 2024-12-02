namespace Jumpeno.Shared.Models;

public class KeyUpdate(ulong id, byte playerID, LinkedList<Control> controls) : NetworkUpdate(id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.KEY_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte PlayerID { get; private set; } = playerID;
    public LinkedList<Control> Controls { get; private set; } = controls;
    
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
