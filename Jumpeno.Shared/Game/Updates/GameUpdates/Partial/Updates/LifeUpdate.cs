namespace Jumpeno.Shared.Models;

public class LifeUpdate(byte playerID) : PartialUpdate {
    public byte PlayerID { get; private set; } = playerID;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
