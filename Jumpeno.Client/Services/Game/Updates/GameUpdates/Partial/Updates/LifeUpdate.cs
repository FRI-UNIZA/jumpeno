namespace Jumpeno.Client.Models;

public class LifeUpdate(byte playerID, double immortalUntil) : PartialUpdate {
    public byte PlayerID { get; private set; } = playerID;
    public double ImmortalUntil { get; set; } = immortalUntil;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
