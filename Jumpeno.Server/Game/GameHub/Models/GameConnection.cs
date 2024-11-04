namespace Jumpeno.Server.Models;

public class GameConnection(string connectionID, GameEngine engine, Player player) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "game-connection";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ConnectionID { get; private set; } = connectionID;
    public GameEngine Engine { get; private set; } = engine;
    public Player Player { get; private set; } = player;
}
