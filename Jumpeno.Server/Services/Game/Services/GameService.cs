namespace Jumpeno.Server.Services;

public static class GameService {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, GameEngine> Engines = new() {{
        Game.DEFAULT_CODE,
        new GameEngine(
            DISPLAY_MODE.ONE_SCREEN, GAME_MODE.MAYHEM,
            User.UNKNOWN, Game.DEFAULT_CODE, Game.DEFAULT_NAME,
            Map.DEFAULT_MAP, Game.MAX_CAPACITY
        )
    }};

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
        return FindEngine(code) ?? throw new CoreException([new Error(Game.CODE_ID, "Game code is incorrect!")])
        .SetCode(400).SetMessage("Game code is incorrect!");
    }

    private static async Task<GameContext> AddClient(GameEngine engine, Connection connection, bool spectator) {
        return spectator ? await engine.AddSpectator(connection) : await engine.AddPlayer(connection);
    }

    // Connection -------------------------------------------------------------------------------------------------------------------------
    public static async Task<GameContext> Connect(string code, Connection connection, bool spectator) {
        // 1) Validation:
        CoreException e = new();
        e.Add(GameValidator.ValidateCode(code));
        e.Add(User.ValidateName(connection.User.Name));
        if (e.HasErrors) throw e;
        // 2) Connect client:
        return await AddClient(AssertEngine(code), connection, spectator);
    }
    public static async Task<GameContext> Connect(GameEngine engine, Connection connection, bool spectator) {
        // 1) Validation:
        var e = new CoreException(User.ValidateName(connection.User.Name));
        if (e.HasErrors) throw e;
        // 2) Connect client:
        return await AddClient(engine, connection, spectator);
    }

    public static async Task Disconnect(string code, Connection connection) => await Disconnect(AssertEngine(code), connection);
    public static async Task Disconnect(GameEngine engine, Connection connection) {
        if (connection is Player player) await engine.RemovePlayer(player);
        else if (connection is Spectator spectator) await engine.RemoveSpectator(spectator);
    }

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public static void Update(string code, GameUpdate update) => Update(AssertEngine(code), update);
    public static void Update(GameEngine engine, GameUpdate update) => engine.Update(update);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task StartGame(string code) => await StartGame(AssertEngine(code));
    public static async Task StartGame(GameEngine engine) => await engine.Start();

    public static async Task PauseGame(string code) => await PauseGame(AssertEngine(code));
    public static async Task PauseGame(GameEngine engine) => await engine.Pause();
    
    public static async Task ResumeGame(string code) => await ResumeGame(AssertEngine(code));
    public static async Task ResumeGame(GameEngine engine) => await engine.Resume();
    
    public static async Task ResetGame(string code) => await ResetGame(AssertEngine(code));
    public static async Task ResetGame(GameEngine engine) => await engine.Reset();
}
