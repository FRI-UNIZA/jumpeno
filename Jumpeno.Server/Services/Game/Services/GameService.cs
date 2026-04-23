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
            Checker.Validate(engine == null, Errors.Invalid.SetID(codeID).SetInfo("Game code is incorrect!")),
            Exceptions.Values
        )!;
    }

    private static void SaveEngine(GameEngine engine) {
        Engines[engine.Game.Code] = engine;
        HostEngines[(Guid)engine.Game.Host.ID!] = engine;
    }

    private static bool RemoveEngine(GameEngine engine) {
        // 1) Assert current engine:
        var current = FindEngine(engine.Game.Code);
        if (current?.Game.ID != engine.Game.ID) return false;
        // 2) Try to remove:
        return Engines.Remove(engine.Game.Code)
        && HostEngines.Remove((Guid)engine.Game.Host.ID!);
    }

    private static bool RemovePlayerEngine(Player player) {
        if (player.User.ID is not Guid id || !player.IsConnected) return false;
        PlayerEngines.Remove(id); return true;
    }

    private static async Task DisposeEngine(GameEngine engine) {
        // 1) Check empty:
        if (!engine.Game.IsEmpty) return;
        // 2) Remove engine:
        RemoveEngine(engine);
        // 3) Dispose engine:
        await engine.DisposeAsync();
    }

    // Connection -------------------------------------------------------------------------------------------------------------------------
    public static async Task<GameContext> Create(GameHubCreateDTO data, Connection connection)
    => await Lock.Exclusive(async () => {
        // 0) Validation:
        data.Assert();
        // 1) Check host:
        if (connection.User.ID is not Guid hostID)
            throw Exceptions.Default.SetInfo("Host must be registered!");
        if (FindHostEngine(hostID) is GameEngine hostEngine)
            throw Exceptions.Default.SetInfo("You already host a game with code \"I18N{code}\"!", new() { ["code"] = hostEngine.Game.Code });
        // 2) Check games limit:
        try { GameValidator.AssertMaxInstances(Engines.Count); }
        catch { throw Exceptions.Server.SetInfo("Maximum games limit exceeded!"); }
        // 3) Obtain code:
        var code = "";
        if (data.Code == null) {
            var g = new StringGenerator();
            do code = g.Generate(GameValidator.CodeLength, Chars.AlphaUpperNum);
            while (Engines.ContainsKey(code));
        } else {
            code = !Engines.ContainsKey(data.Code) ? data.Code :
            throw Exceptions.Values.SetInfo("Game code already exists!").SetErrors(Errors.Exists.SetID(nameof(data.Code)));
        }
        // 4) Select map:
        var map = MapType.ByID(data.Map ?? 0, nameof(data.Map));
        // 5) Create engine:
        var engine = new GameEngine(
            data.DisplayMode, data.GameMode,
            connection.User, code, data.GameName,
            map, data.Anonyms, data.Rounds, data.Capacity
        );
        // 6) Connect:
        var ctx =  await Connect(engine, connection, data.DisplayMode == DisplayMode.Presentation);
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
            if (connection.User.ID is Guid playerID) {
                if (FindHostEngine(playerID) is GameEngine hostEngine && playerID != engine.Game.Host.ID)
                    throw Exceptions.Default.SetInfo("You already host a game with code \"I18N{code}\"!", new() { ["code"] = hostEngine.Game.Code });
                if (FindPlayerEngine(playerID) is GameEngine playerEngine)
                    throw Exceptions.Default.SetInfo("You already play a game with code \"I18N{code}\"!", new() { ["code"] = playerEngine.Game.Code });
            }
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
    => await Lock.Exclusive(() => Connect(AssertEngine(code, codeID), connection, spectator, nameID));

    public static async Task Disconnect(GameContext ctx)
    => await Lock.Exclusive(async () => {
        try {
            // 1) Disconnect:
            switch (ctx.Connection) {
                // 1.1) Spectator:
                case Spectator spectator:
                    await ctx.Engine.RemoveSpectator(spectator);
                break;
                // 1.2) Player:
                case Player player:
                    // 1.2.1) Remove player (can be invalidated):
                    var removedPlayer = await ctx.Engine.RemovePlayer(player);
                    // 1.2.2) Remove registered player engine:
                    RemovePlayerEngine(removedPlayer);
                break;
            }
        } finally {
            // 2) Dispose engine:
            await DisposeEngine(ctx.Engine);
        }
    });

    // Updates ----------------------------------------------------------------------------------------------------------------------------
    private static void Update(GameEngine engine, GameUpdate update) => engine.Update(update);
    public static void Update(GameContext ctx, GameUpdate update) => Update(ctx.Engine, update);
    public static void Update(
        // Parameters:
        string code, GameUpdate update,
        // Exceptions:
        string codeID = ""
    )
    => Lock.Exclusive(() => Update(AssertEngine(code, codeID), update));

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private static async Task StartGame(GameEngine engine, GameContext? ctx = null) => await engine.Start(ctx);
    public static async Task StartGame(GameContext ctx) => await StartGame(ctx.Engine, ctx);
    public static async Task StartGame(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(() => StartGame(AssertEngine(code, codeID)));

    private static async Task PauseGame(GameEngine engine, GameContext? ctx = null) => await engine.Pause(ctx);
    public static async Task PauseGame(GameContext ctx) => await PauseGame(ctx.Engine, ctx);
    public static async Task PauseGame(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(() => PauseGame(AssertEngine(code, codeID)));

    private static async Task ToggleGame(GameEngine engine, GameContext? ctx = null) => await engine.Toggle(ctx);
    public static async Task ToggleGame(GameContext ctx) => await ToggleGame(ctx.Engine, ctx);
    public static async Task ToggleGame(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(() => ToggleGame(AssertEngine(code, codeID)));

    private static async Task DeleteGame(
        // Parameters:
        GameEngine engine,
        // Exceptions:
        string codeID = ""
    ) {
        // 1.1) Remove engine:
        if (RemoveEngine(engine)) await engine.Delete();
        // 1.2) Or throw exception:
        else throw Exceptions.Values.SetInfo("Game code is incorrect!")
        .SetErrors(Errors.Invalid.SetID(codeID).SetInfo("Game code is incorrect!"));
    }
    public static async Task DeleteGame(GameContext ctx) => await Lock.Exclusive(() => DeleteGame(ctx.Engine));
    public static async Task DeleteGame(
        // Parameters:
        string code,
        // Exceptions:
        string codeID = ""
    )
    => await Lock.Exclusive(() => DeleteGame(AssertEngine(code, codeID), codeID));

    // Players > Ready --------------------------------------------------------------------------------------------------------------------
    public static async Task SetPlayerReady(GameContext ctx) => await ctx.Engine.SetPlayerReady(ctx);

    private static async Task SetPlayerReadyByName(
        // Parameters:
        GameEngine engine, string name,
        // Response:
        GameContext? ctx = null,
        // Exceptions:
        string nameID = ""
    )
    => await engine.SetPlayerReadyByName(name, ctx, nameID);
    public static async Task SetPlayerReadyByName(
        // Parameters:
        GameContext ctx, string name,
        // Exceptions:
        string nameID = ""
    )
    => await SetPlayerReadyByName(ctx.Engine, name, ctx, nameID);
    public static async Task SetPlayerReadyByName(
        // Parameters:
        string code, string name,
        // Exceptions:
        string codeID = "", string nameID = ""
    )
    => await Lock.Exclusive(() => SetPlayerReadyByName(AssertEngine(code, codeID), name, null, nameID));

    // Players > Kick ---------------------------------------------------------------------------------------------------------------------
    private static async Task KickPlayerByName(
        // Parameters:
        GameEngine engine, string name,
        // Response:
        GameContext? ctx = null,
        // Exceptions:
        string nameID = ""
    ) {
        try {
            // 1) Kick player (will be invalidated):
            var kickedPlayer = await engine.KickPlayerByName(name, ctx, nameID);
            // 2) Remove registered player engine:
            RemovePlayerEngine(kickedPlayer);
        } finally {
            // 3) Dispose engine:
            await DisposeEngine(engine);
        }
    }
    public static async Task KickPlayerByName(
        // Parameters:
        GameContext ctx, string name,
        // Exceptions:
        string nameID = ""
    )
    => await Lock.Exclusive(() => KickPlayerByName(ctx.Engine, name, ctx, nameID));
    public static async Task KickPlayerByName(
        // Parameters:
        string code, string name,
        // Exceptions:
        string codeID = "", string nameID = ""
    )
    => await Lock.Exclusive(() => KickPlayerByName(AssertEngine(code, codeID), name, null, nameID));
}
