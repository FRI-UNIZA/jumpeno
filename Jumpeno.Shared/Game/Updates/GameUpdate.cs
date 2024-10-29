namespace Jumpeno.Shared.Models;

public class GameUpdate(double time, GAME_STATE state) {
    public double Time { get; private set; } = time;
    public GAME_STATE State { get; private set; } = state;
}
