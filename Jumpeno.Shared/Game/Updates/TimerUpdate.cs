namespace Jumpeno.Shared.Models;

public class TimerUpdate(ulong id, double time, TIMER_STATE state): GameUpdate(id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public override string HUB_ACTION => GAME_HUB.TIMER_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public double Time { get; private set; } = time;
    public TIMER_STATE State { get; private set; } = state;
}
