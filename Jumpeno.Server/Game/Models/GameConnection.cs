namespace Jumpeno.Server.Models;

public class GameConnection(GameEngine engine, Player player) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "game-connection";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GameEngine GameEngine { get; private set; } = engine;
    public Player Player { get; private set; } = player;
}
