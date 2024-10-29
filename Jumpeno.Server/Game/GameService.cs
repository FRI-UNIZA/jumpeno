namespace Jumpeno.Server.Services;

public static class GameService {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, GameEngine> Engines = new() {
        {"FRI25", new GameEngine("FRI25", 2)}
    };

    // Locks ------------------------------------------------------------------------------------------------------------------------------
    private static readonly Semaphore Lock = new(1, 1);

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static GameEngine? FindEngine(string code) {
        try {
            Lock.WaitOne();
            Engines.TryGetValue(code, out var engine);
            return engine;   
        } finally {
            Lock.Release();
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task<GameConnection> Connect(string connectionID, string code, User user) {
        // 0) Results:
        GameEngine? engine = null;
        Player? player = null;
        GameException exception = new();
        // 1) Validation:
        exception.Add(Game.ValidateCode(code));
        exception.Add(User.ValidateName(user.Name));
        // 2) Find game:
        if (!exception.HasErrors()) {
            engine = FindEngine(code);
            if (engine != null) {
                // 2.1) Add client to game:
                try { player = await engine.AddPlayer(connectionID, user); }
                catch (Exception e) { exception.Add(new Error(e.Message)); }
            } else {
                // 2.2) Game does not exist:
                exception.Add(new Error(Game.CODE_ID, I18N.T("Game code is incorrect!")));
            }
        }
        // 3) Return results:
        if (exception.HasErrors()) throw exception;
        return new(engine!, player!);
    }

    public static async Task Disconnect(GameConnection connection) {
        await connection.GameEngine.RemovePlayer(connection.Player);
    }
}
