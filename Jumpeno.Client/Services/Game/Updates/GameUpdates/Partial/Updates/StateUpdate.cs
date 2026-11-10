namespace Jumpeno.Client.Models;

public class StateUpdate(double time, GameStates state, int level, double timer) : PartialUpdate {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Game:
    public double Time { get; private set; } = time;
    public GameStates State { get; private set; } = state;
    // Shrink:
    public int Level { get; private set; } = level;
    public double Timer { get; private set; } = timer;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
