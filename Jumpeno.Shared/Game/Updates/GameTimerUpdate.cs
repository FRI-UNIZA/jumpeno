namespace Jumpeno.Shared.Models;

public class GameTimerUpdate(double time, GAME_STATE state, TIMER_STATE timerState) : GameUpdate(time, state) {
    public TIMER_STATE TimerState { get; private set; } = timerState;
}
