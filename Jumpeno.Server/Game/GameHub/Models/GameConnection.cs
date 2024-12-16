namespace Jumpeno.Server.Models;

public class GameConnection(string connectionID, GameEngine engine, Spectator spectator) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "game-connection";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ConnectionID { get; private set; } = connectionID;
    public GameEngine Engine { get; private set; } = engine;
    public Spectator Spectator { get; private set; } = spectator;
}
