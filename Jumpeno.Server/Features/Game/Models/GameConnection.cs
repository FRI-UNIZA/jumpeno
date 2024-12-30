namespace Jumpeno.Server.Models;

public class GameConnection {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "game-connection";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GameEngine Engine { get; private set; }
    public Connection Connection { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameConnection(GameEngine engine, Connection connection) {
        if (connection is not Spectator && connection is not Player) {   
            throw new ArgumentException("Connection type invalid!");
        }
        Engine = engine;
        Connection = connection;
    }
}
