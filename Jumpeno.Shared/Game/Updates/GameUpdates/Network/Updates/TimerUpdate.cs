namespace Jumpeno.Shared.Models;

public class TimerUpdate(ulong id, double time, TIMER_STATE state) : NetworkUpdate(id) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    [JsonIgnore]
    public override string HUB_ACTION => GAME_HUB.TIMER_UPDATE;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public double Time { get; private set; } = time;
    public TIMER_STATE State { get; private set; } = state;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public override string ToString() => Format.JSON_PRETTY(this);
}
