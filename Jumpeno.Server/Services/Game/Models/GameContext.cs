namespace Jumpeno.Server.Models;

public class GameContext {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string TCS = "connect-tcs";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public GameEngine Engine { get; private set; }
    public Connection Connection { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    private GameContext(GameEngine engine, Connection connection) {
        Engine = engine;
        Connection = connection;
    }
    public GameContext(GameEngine engine, Player connection) : this(engine, (Connection)connection) {}
    public GameContext(GameEngine engine, Spectator connection) : this(engine, (Connection)connection) {}
}
