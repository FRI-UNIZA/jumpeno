namespace Jumpeno.Server.Services;

public static class GameService {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, GameEngine> Engines = new() {
        {Game.MOCK_CODE, new GameEngine(DISPLAY_MODE.ONE_SCREEN, User.UNKNOWN, Game.MOCK_CODE, Game.MOCK_NAME, 5)}
    };

    // Locks ------------------------------------------------------------------------------------------------------------------------------
    private static readonly Locker Lock = new();

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static GameEngine? FindEngine(string code) {
        return Lock.Exclusive(() => {
            Engines.TryGetValue(code, out var engine);
            return engine;  
        });
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task<(GameEngine Engine, Spectator Spectator)> Connect(string code, User user, DEVICE_TYPE device, bool watch) {
        // 1) Results:
        GameEngine? engine = null;
        Spectator? spectator = null;
        CoreException exception = new();
        // 2) Validation:
        exception.Add(Game.ValidateCode(code));
        exception.Add(User.ValidateName(user.Name));
        // 3) Find game:
        if (!exception.HasErrors) {
            engine = FindEngine(code);
            if (engine != null) {
                // 3.1) Add client to game:
                try { spectator = watch ? await engine.AddSpectator(user, device) : await engine.AddPlayer(user, device); }
                catch (Exception e) { exception.Add(new Error(e.Message)); }
            } else {
                // 3.2) Game does not exist:
                exception.Add(new Error(Game.CODE_ID, "Game code is incorrect!"));
            }
        }
        // 4) Return results:
        if (exception.HasErrors) throw exception;
        return new(engine!, spectator!);
    }

    public static async Task Disconnect(GameEngine engine, Spectator spectator) {
        if (spectator is Player player) await engine.RemovePlayer(player);
        else await engine.RemoveSpectator(spectator);
    }

    public static void Update(GameEngine engine, GameUpdate update) {
        engine.Update(update);
    }

    public static async Task StartGame(string code) {
        var engine = FindEngine(code) ?? throw new CoreException(new Error(Game.CODE_ID, "Game code is incorrect!"));
        await engine.Start();
    }

    public static async Task ResetGame(string code) {
        var engine = FindEngine(code) ?? throw new CoreException(new Error(Game.CODE_ID, "Game code is incorrect!"));
        await engine.Reset();
    }
}
