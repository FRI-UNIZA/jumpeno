namespace Jumpeno.Shared.Models;

public class GamePlayUpdate(double time, GAME_STATE state, List<AvatarUpdate> updates) : GameUpdate(time, state) {
    public List<AvatarUpdate> Updates { get; private set; } = updates;
}
