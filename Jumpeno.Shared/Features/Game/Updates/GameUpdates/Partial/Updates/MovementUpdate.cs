namespace Jumpeno.Shared.Models;

public class MovementUpdate : PartialUpdate {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte PlayerID { get; private set; }
    public PointF Center { get; private set; }
    public PointF Direction { get; private set; }
    public float? JumpFinishY { get; private set; }
    public PointF? Normal { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public MovementUpdate(byte playerID, PointF center, PointF direction, float? jumpFinishY, PointF? normal = null) {
        PlayerID = playerID;
        Center = center;
        Direction = direction;
        JumpFinishY = jumpFinishY;
        Normal = normal;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
