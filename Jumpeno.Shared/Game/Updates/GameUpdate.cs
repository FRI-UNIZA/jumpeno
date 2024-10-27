namespace Jumpeno.Shared.Models;

public class GameUpdate {
    public double Time { get; private set; }
    public GAME_STATE State { get; private set; }
    public List<PlayerUpdate> Updates { get; private set; }
}
