namespace Jumpeno.Shared.Models;

public class LifeUpdate(Player player, double immortalMS = 0) : PartialUpdate {
    public Player Player { get; private set; } = player;
    public double ImmortalMS { get; private set; } = immortalMS;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
