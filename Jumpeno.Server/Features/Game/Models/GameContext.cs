namespace Jumpeno.Server.Models;

public class GameContext {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "game-context";
    public const string TCS_ID = "connect-tcs";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GameEngine Engine { get; private set; }
    public Connection Connection { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameContext(GameEngine engine, Connection connection) {
        if (connection is not Spectator && connection is not Player)
        throw new ArgumentException("Connection type invalid!");
        Engine = engine;
        Connection = connection;
    }
}
