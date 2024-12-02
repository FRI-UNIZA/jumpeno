namespace Jumpeno.Shared.Models;

public class KillUpdate(byte? killerID, byte deadID) : PartialUpdate {
    public byte? KillerID { get; private set; } = killerID;
    public byte DeadID { get; private set; } = deadID;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
