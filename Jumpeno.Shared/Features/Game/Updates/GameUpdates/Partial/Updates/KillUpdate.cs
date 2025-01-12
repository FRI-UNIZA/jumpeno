namespace Jumpeno.Shared.Models;

public class KillUpdate(byte? killerID, byte deadID, bool penalize) : PartialUpdate {
    public byte? KillerID { get; private set; } = killerID;
    public byte DeadID { get; private set; } = deadID;
    public bool Penalize { get; private set; } = penalize;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
