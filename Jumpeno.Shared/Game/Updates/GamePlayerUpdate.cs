namespace Jumpeno.Shared.Models;

public class GamePlayerUpdate(double time, GAME_STATE state, Player player, PLAYER_ACTION action) : GameUpdate(time, state) {
    public Player Player { get; private set; } = player;
    public PLAYER_ACTION Action { get; private set; } = action;
}
