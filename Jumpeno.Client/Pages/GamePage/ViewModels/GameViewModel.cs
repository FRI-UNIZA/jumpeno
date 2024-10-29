namespace Jumpeno.Client.ViewModels;

public class GameViewModel {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Game Game { get; private set; }
    public Player Player { get; private set; }
    public Func<string, object, Task> Send { get; private set; }
    public EmptyDelegate OnRender { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameViewModel(Game game, Player player, Func<string, object, Task> send, EmptyDelegate onRender) {
        Game = game;
        Player = GamePlayer(player);
        Send = send;
        OnRender = onRender;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private Player GamePlayer(Player player) {
        foreach (var gamePlayer in Game.PlayersIterator) {
            if (player.Equals(gamePlayer)) return gamePlayer;
        }
        return player;
    }
}
