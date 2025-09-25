namespace Jumpeno.Client.Models;

public class StateUpdate(double time, GAME_STATE state, int level, double timer) : PartialUpdate {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Game:
    public double Time { get; private set; } = time;
    public GAME_STATE State { get; private set; } = state;
    // Shrink:
    public int Level { get; private set; } = level;
    public double Timer { get; private set; } = timer;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
