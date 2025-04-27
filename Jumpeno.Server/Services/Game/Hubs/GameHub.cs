namespace Jumpeno.Server.Hubs;

#pragma warning disable CS1998

public class GameHub : Hub {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(WebApplication app) {
        app.MapHub<GameHub>(GAME_HUB.ROUTE_EN);
        app.MapHub<GameHub>(GAME_HUB.ROUTE_SK);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static IHubContext<GameHub> Hub => AppEnvironment.GetService<IHubContext<GameHub>>();
    private GameContext? GameContext {
        get { try { return (GameContext) Context.Items[nameof(GameContext)]!; } catch { return null; } }
        set { Context.Items[nameof(GameContext)] = value; }
    }
    private TaskCompletionSource? ConnectTCS {
        get { try { return (TaskCompletionSource) Context.Items[GameContext.TCS]!; } catch { return null; } }
        set { Context.Items[GameContext.TCS] = value; }
    }

    // Groups -----------------------------------------------------------------------------------------------------------------------------
    private static string GroupName(string code, UPDATE_GROUP group) => $"{code}-{group}";
    private static List<string> GroupNames(GameContext ctx) {
        var code = ctx.Engine.Game.Code;
        // 1) Common group:
        List<string> groups = [GroupName(code, UPDATE_GROUP.ALL)];
        // 2) All spectators:
        if (
            ctx.Engine.Game.DisplayMode == DISPLAY_MODE.ONE_SCREEN
            && ctx.Connection is Player
            && !ctx.Connection.User.Equals(ctx.Engine.Game.Host)
        ) return groups;
        groups.Add(GroupName(code, UPDATE_GROUP.WATCH));
        // 3) Touch spectators:
        if (ctx.Connection.Device != DEVICE_TYPE.TOUCH) return groups;
        groups.Add(GroupName(code, UPDATE_GROUP.WATCH_TOUCH));
        return groups;
    }
    private static async Task AddToGroups(GameContext ctx) {
        foreach (var group in GroupNames(ctx)) {
            if (ctx.Connection.ConnectionID is not string connectionID) return;
            await Hub.Groups.AddToGroupAsync(connectionID, group);
        }
    }
    private static async Task RemoveFromGroups(GameContext ctx) {
        foreach (var group in GroupNames(ctx)) {
            if (ctx.Connection.ConnectionID is not string connectionID) return;
            await Hub.Groups.RemoveFromGroupAsync(connectionID, group);
        }
    }

    // Exceptions -------------------------------------------------------------------------------------------------------------------------
    private static async Task HandleException(IClientProxy proxy, Exception e) {
        await proxy.SendAsync(GAME_HUB.ERROR, (e is CoreException exception ? exception : new CoreException()).DTO);
        // NOTE: Client must close the connection!
    }

    private async Task HandleCallException(Exception e) {
        await HandleException(Clients.Caller, e);
        // NOTE: Automatically closes connection:
        Context.Abort();
    }

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    private (string Code, User user, DEVICE_TYPE Device, bool Spectator) ReadConnectParams() {
        // 1) Context:
        var ctx = Context.GetHttpContext() ?? throw new CoreException(new Error("Missing parameters!"));
        // 2.1) Code:
        if (!ctx.Request.Query.TryGetValue(GameValidator.CODE, out var queryCode))
            throw new CoreException(new Error(GameValidator.CODE, "Code not set!"));
        var code = $"{queryCode}";
        // 2.2) User:
        if (!ctx.Request.Query.TryGetValue(nameof(User), out var queryUser))
            throw new CoreException(new Error(nameof(User), "User undefined!"));
        var user = JsonSerializer.Deserialize<User>(queryUser!)!;
        // 2.3) Touch device:
        if (!ctx.Request.Query.TryGetValue(nameof(Connection.Device), out var queryDevice))
            throw new CoreException(new Error(nameof(Connection.Device), "Device parameter not set!"));
        var device = JsonSerializer.Deserialize<DEVICE_TYPE>(queryDevice!)!;
        // 2.4) Spectator:
        if (!ctx.Request.Query.TryGetValue(nameof(Spectator), out var querySpectator))
            throw new CoreException(new Error(nameof(Spectator), "Spectator parameter not set!"));
        var spectator = bool.Parse(querySpectator!);
        // 3) Return:
        return (code, user, device, spectator);
    }

    // Connect ----------------------------------------------------------------------------------------------------------------------------
    public static async Task BeforeConnected() {}
    public override async Task OnConnectedAsync() {
        try {
            // 1) Create TCS:
            ConnectTCS = new();
            // 2) Query parameters:
            var (code, user, device, spectator) = ReadConnectParams();
            // 3) Connect to game:
            // NOTE: [Locked] BeforeConnected()
            GameContext = await GameService.Connect(code, new(Context.ConnectionId, user, device), spectator);
            // NOTE: [Locked] AfterConnected(GameContext)
        } catch (Exception e) {
            // 4) Handle error:
            await HandleCallException(e);
        } finally {
            // 5) Set result on TCS:
            ConnectTCS?.TrySetResult();
        }
    }
    public static async Task AfterConnected(GameContext ctx) {
        if (ctx.Connection.ConnectionID is not string id) return;
        // 1) Add to groups:
        await AddToGroups(ctx);
        // 2) Send response:
        await Hub.Clients.Client(id).SendAsync(
            GAME_HUB.CONNECTION_SUCCESSFUL, ctx.Engine.Game, ctx.Connection is Player player ? player : null
        );
    }

    // Client updates ---------------------------------------------------------------------------------------------------------------------
    public void KeyUpdate(KeyUpdate update) {
        try {
            if (GameContext is null) return;
            if (GameContext.Engine.Game.GetPlayerRef(update.PlayerID) != GameContext.Connection) return;
            GameService.Update(GameContext.Engine, update);
        } catch (Exception e) {
            Console.Error.WriteLine(e);
        }
    }

    public async Task PingUpdate(PingUpdate update) {
        try { await Clients.Caller.SendAsync(update.HUB_ACTION, update); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    // Server updates ---------------------------------------------------------------------------------------------------------------------
    public static async Task SendGameUpdate(Game game, UPDATE_GROUP group, NetworkUpdate update) {
        try { await Hub.Clients.Group(GroupName(game.Code, group)).SendAsync(update.HUB_ACTION, update); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    public static async Task SendGameUpdate(Connection? connection, NetworkUpdate update) {
        try {
            if (connection == null || connection.ConnectionID is not string id) return;
            await Hub.Clients.Client(id).SendAsync(update.HUB_ACTION, update);
        }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    public static async Task SendException(Game game, UPDATE_GROUP group, CoreException exception) {
        try { await HandleException(Hub.Clients.Group(GroupName(game.Code, group)), exception); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    public static async Task SendException(Connection? connection, CoreException exception) {
        try {
            if (connection == null || connection.ConnectionID is not string id) return;
            await HandleException(Hub.Clients.Client(id), exception);
        }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    // Disconnect -------------------------------------------------------------------------------------------------------------------------
    public static async Task BeforeDisconnected(GameContext ctx) {
        // Remove disconnected client from groups:
        await RemoveFromGroups(ctx);
    }
    public override async Task OnDisconnectedAsync(Exception? exception) {
        try {
            // 1) Wait to connect:
            if (ConnectTCS != null) await ConnectTCS.Task;
            // 2) Check context:
            if (GameContext == null) return;
            // 3) Disconnect player from the game:
            // NOTE: [Locked] BeforeDisconnected(GameContext)
            await GameService.Disconnect(GameContext.Engine, GameContext.Connection);
            // NOTE: [Locked] AfterDisconnected(GameContext)
        } catch (Exception e) {
            // 4) Handle error:
            Console.Error.WriteLine(e);
        }
    }
    public static async Task AfterDisconnected(GameContext ctx) {}
}
