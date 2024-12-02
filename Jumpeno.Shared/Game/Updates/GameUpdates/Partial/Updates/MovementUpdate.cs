namespace Jumpeno.Shared.Models;

public class MovementUpdate : PartialUpdate {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public byte PlayerID { get; private set; }
    public List<ulong> KeyUpdateIDs { get; private set; }
    public PointF Center { get; private set; }
    public PointF Direction { get; private set; }
    public float? JumpFinishY { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    public MovementUpdate(byte playerID, List<ulong> keyUpdateIDs, PointF center, PointF direction, float? jumpFinishY) {
        PlayerID = playerID;
        KeyUpdateIDs = keyUpdateIDs;
        Center = center;
        Direction = direction;
        JumpFinishY = jumpFinishY;
    }

    public MovementUpdate(byte playerID, ulong? keyUpdateID, PointF center, PointF direction, float? jumpFinishY):
    this(playerID, [], center, direction, jumpFinishY) {
        if (keyUpdateID is ulong id) KeyUpdateIDs.Add(id);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public void Chain(MovementUpdate previous) {
        KeyUpdateIDs.InsertRange(0, previous.KeyUpdateIDs);
    }

    public override string ToString() => Format.JSON_PRETTY(this);
}
