namespace Jumpeno.Server.Services;

public static class GameService {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, GameEngine> Engines = new() {
        {Game.MOCK_CODE, new GameEngine(DISPLAY_MODE.ONE_SCREEN, GAME_MODE.MAYHEM, User.UNKNOWN, Game.MOCK_CODE, Game.MOCK_NAME, 10)}
    };

    // Locks ------------------------------------------------------------------------------------------------------------------------------
    private static readonly Locker Lock = new();

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private static GameEngine? FindEngine(string code) {
        return Lock.Exclusive(() => {
            Engines.TryGetValue(code, out var engine);
            return engine;  
        });
    }

    private static GameEngine AssertEngine(string code) {
        return FindEngine(code) ?? throw new CoreException(new Error(Game.CODE_ID, "Game code is incorrect!"));
    }

    // Connection -------------------------------------------------------------------------------------------------------------------------
    public static async Task<GameConnection> Connect(string code, Connection connection, bool spectator) {
        // 1) Results:
        GameEngine? engine = null;
        Connection? result = null;
        CoreException exception = new();
        // 2) Validation:
        exception.Add(Game.ValidateCode(code));
        exception.Add(User.ValidateName(connection.User.Name));
        // 3) Find game:
        if (!exception.HasErrors) {
            engine = FindEngine(code);
            if (engine != null) {
                // 3.1) Add client to game:
                try { result = spectator ? await engine.AddSpectator(connection) : await engine.AddPlayer(connection); }
                catch (Exception e) { exception.Add(new Error(e.Message)); }
            } else {
                // 3.2) Game does not exist:
                exception.Add(new Error(Game.CODE_ID, "Game code is incorrect!"));
            }
        }
        // 4) Return results:
        if (exception.HasErrors) throw exception;
        return new(engine!, result!);
    }

    public static async Task Disconnect(GameEngine engine, Connection connection) {
        if (connection is Player player) await engine.RemovePlayer(player);
        else if (connection is Spectator spectator) await engine.RemoveSpectator(spectator);
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public static void Update(GameEngine engine, GameUpdate update) {
        engine.Update(update);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task StartGame(string code) {
        var engine = AssertEngine(code);
        await engine.Start();
    }

    public static async Task PauseGame(string code) {
        var engine = AssertEngine(code);
        await engine.Pause();
    }

    public static async Task ResumeGame(string code) {
        var engine = AssertEngine(code);
        await engine.Resume();
    }

    public static async Task ResetGame(string code) {
        var engine = AssertEngine(code);
        await engine.Reset();
    }
}
