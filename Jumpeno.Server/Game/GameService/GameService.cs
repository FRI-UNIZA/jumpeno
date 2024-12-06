namespace Jumpeno.Server.Services;

public static class GameService {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, GameEngine> Engines = new() {
        {Game.MOCK_CODE, new GameEngine(ACCESS_MODE.EACH_OWN, Game.MOCK_CODE, Game.MOCK_NAME, 5)}
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
    public static async Task<EnginePlayer> Connect(string code, User user, bool touchDevice = false) {
        // 1) Results:
        GameEngine? engine = null;
        Player? player = null;
        GameException exception = new();
        // 2) Validation:
        exception.Add(Game.ValidateCode(code));
        exception.Add(User.ValidateName(user.Name));
        // 3) Find game:
        if (!exception.HasErrors()) {
            engine = FindEngine(code);
            if (engine != null) {
                // 3.1) Add client to game:
                try { player = await engine.AddPlayer(user, touchDevice); }
                catch (Exception e) { exception.Add(new Error(e.Message)); }
            } else {
                // 3.2) Game does not exist:
                exception.Add(new Error(Game.CODE_ID, I18N.T("Game code is incorrect!")));
            }
        }
        // 4) Return results:
        if (exception.HasErrors()) throw exception;
        return new(engine!, player!);
    }

    public static async Task Disconnect(GameEngine engine, Player player) {
        await engine.RemovePlayer(player);
    }

    public static void Update(GameEngine engine, GameUpdate update) {
        engine.Update(update);
    }

    public static async Task StartGame(string code) {
        var engine = FindEngine(code) ?? throw new GameException([new(Game.CODE_ID, I18N.T("Game code is incorrect!"))]);
        await engine.Start();
    }

    public static async Task ResetGame(string code) {
        var engine = FindEngine(code) ?? throw new GameException([new(Game.CODE_ID, I18N.T("Game code is incorrect!"))]);
        await engine.Reset();
    }
}
