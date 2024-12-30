namespace Jumpeno.Shared.Models;

public class StateUpdate(double time, GAME_STATE state) : PartialUpdate {
    public double Time { get; private set; } = time;
    public GAME_STATE State { get; private set; } = state;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
