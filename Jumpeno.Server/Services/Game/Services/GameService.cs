namespace Jumpeno.Server.Services;

public static class GameService {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, GameEngine> Engines = []; // By game code
    private static readonly Dictionary<Guid, GameEngine> HostEngines = []; // By host ID
    private static readonly Dictionary<Guid, GameEngine> PlayerEngines = []; // By player ID

    // Locks ------------------------------------------------------------------------------------------------------------------------------
    private static readonly Locker Lock = new();

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private static GameEngine? FindEngine(string code) => Engines.GetValueOrDefault(code);
    private static GameEngine? FindHostEngine(Guid id) => HostEngines.GetValueOrDefault(id);
    private static GameEngine? FindPlayerEngine(Guid id) => PlayerEngines.GetValueOrDefault(id);
    private static GameEngine AssertEngine(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    ) {
        var engine = FindEngine(code);
        return Checker.Assert(
            engine,
            Checker.Validate(engine == null, ERROR.INVALID.SetID(codeID).SetInfo("Game code is incorrect!")),
            EXCEPTION.VALUES
        )!;
    }

    private static void SaveEngine(GameEngine engine) {
        Engines[engine.Game.Code] = engine;
        HostEngines[(Guid)engine.Game.Host.ID!] = engine;
    }

    private static bool RemoveEngine(GameEngine engine) {
        return Engines.Remove(engine.Game.Code)
        && HostEngines.Remove((Guid)engine.Game.Host.ID!);
    }

    // Connection -------------------------------------------------------------------------------------------------------------------------
    public static async Task<GameContext> Create(GameHubCreateDTO data, Connection connection)
    => await Lock.Exclusive(async () => {
        // 0) Validation:
        data.Assert();
        // 1) Check host:
        if (connection.User.ID is not Guid hostID)
            throw EXCEPTION.DEFAULT.SetInfo("Host must be registered!"); 
        if (FindHostEngine(hostID) is GameEngine hostEngine)
            throw EXCEPTION.DEFAULT.SetInfo("You already host a game with code \"I18N{code}\"!", new() { ["code"] = hostEngine.Game.Code });
        // 2) Check games limit:
        try { GameValidator.AssertMaxInstances(Engines.Count); }
        catch { throw EXCEPTION.SERVER.SetInfo("Maximum games limit exceeded!"); }
        // 3) Obtain code:
        var code = "";
        if (data.Code == null) {
            var g = new StringGenerator();
            do code = g.Generate(GameValidator.CODE_LENGTH, CHARS.ALPHA_UPPER_NUM);
            while (Engines.ContainsKey(code));
        } else {
            code = !Engines.ContainsKey(data.Code) ? data.Code :
            throw EXCEPTION.VALUES.SetInfo("Game code already exists!").SetErrors(ERROR.EXISTS.SetID(nameof(data.Code)));
        }
        // 4) Select map:
        var map = MAP.ByID(data.Map ?? 0, nameof(data.Map));
        // 5) Create engine:
        var engine = new GameEngine(
            data.DisplayMode, data.GameMode,
            connection.User, code, data.GameName,
            map, data.Anonyms, data.Rounds, data.Capacity
        );
        // 6) Connect:
        var ctx =  await Connect(engine, connection, data.DisplayMode == DISPLAY_MODE.PRESENTATION);
        // 7) Save engine:
        SaveEngine(engine);
        // 8) Return context:
        return ctx;
    });

    private static async Task<GameContext> Connect(
        // Parameters:
        GameEngine engine, Connection connection, bool spectator,
        // Exceptions:
        string nameID = ""
    ) {
        // 1) Spectator:
        if (spectator) return await engine.AddSpectator(connection, nameID);
        // 2) Player:
        else {
            // 2.1) Check registered player:
            if (connection.User.ID is Guid playerID && FindPlayerEngine(playerID) is GameEngine playerEngine)
                throw EXCEPTION.DEFAULT.SetInfo("You already play a game with code \"I18N{code}\"!", new() { ["code"] = playerEngine.Game.Code });
            // 2.2) Add player:
            var ctx = await engine.AddPlayer(connection, nameID);
            // 2.3) Add registered player engine:
            if (connection.User.ID is Guid id) PlayerEngines[id] = engine;
            // 2.4) Return context:
            return ctx;
        }
    }
    public static async Task<GameContext> Connect(
        // Parameters:
        string code, Connection connection, bool spectator,
        // Exceptions:
        string codeID = "", string nameID = ""
    )
    => await Lock.Exclusive(async () =>
        await Connect(AssertEngine(code, codeID), connection, spectator, nameID)
    );

    public static async Task Disconnect(GameEngine engine, Connection connection)
    => await Lock.Exclusive(async () => {
        try {
            // 1) Connect:
            switch (connection) {
                // 1.1) Spectator:
                case Spectator spectator:
                    await engine.RemoveSpectator(spectator);
                break;
                // 1.2) Player:
                case Player player:
                    // 1.2.1) Remove player:
                    await engine.RemovePlayer(player);
                    // 1.2.2) Remove registered player engine:
                    if (player.User.ID is Guid playerID) PlayerEngines.Remove(playerID);
                break;
            }
        } finally {
            // 2) Dispose engine:
            if (engine.Game.IsEmpty) {
                RemoveEngine(engine);
                await engine.DisposeAsync();
            }
        }
    });

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    public static void Update(GameEngine engine, GameUpdate update) => engine.Update(update);
    public static void Update(
        // Parameters:
        string code, GameUpdate update,
        // Exceptions:
        string codeID = ""
    )
    => Lock.Exclusive(() => {
        Update(AssertEngine(code, codeID), update); 
    });

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task StartGame(GameEngine engine) => await engine.Start();
    public static async Task StartGame(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(async () => {
        await StartGame(AssertEngine(code, codeID));
    });

    public static async Task PauseGame(GameEngine engine) => await engine.Pause();
    public static async Task PauseGame(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(async () => {
        await PauseGame(AssertEngine(code, codeID));
    });

    public static async Task ToggleGame(GameEngine engine) => await engine.Toggle();
    public static async Task ToggleGame(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(async () => {
        await ToggleGame(AssertEngine(code, codeID));
    });

    private static async Task Delete(
        // Parameters:
        GameEngine engine,
        // Exceptions:
        string codeID = ""
    ) {
        // 1.1) Remove engine:
        if (RemoveEngine(engine)) await engine.Delete();
        // 1.2) Or throw exception:
        else throw EXCEPTION.VALUES.SetErrors(ERROR.INVALID.SetID(codeID).SetInfo("Game code is incorrect!"));
    }
    public static async Task DeleteGame(
        // Parameters:
        GameEngine engine,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(async () => {
        await Delete(engine, codeID);
    });
    public static async Task DeleteGame(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(async () => {
        await Delete(AssertEngine(code, codeID), codeID);
    });
}
