namespace Jumpeno.Shared.Models;

public class TimeFlowUpdate(Game game, double deltaT) : PartialUpdate {
    public Game Game { get; private set; } = game;
    public double DeltaT { get; private set; } = deltaT;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
