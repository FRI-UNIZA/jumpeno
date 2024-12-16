namespace Jumpeno.Server.Hubs;

public class GameHub : Hub {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(WebApplication app) {
        app.MapHub<GameHub>(GAME_HUB.ROUTE_EN);
        app.MapHub<GameHub>(GAME_HUB.ROUTE_SK);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static IHubContext<GameHub> Hub => AppEnvironment.GetService<IHubContext<GameHub>>();
    private GameConnection? GameConnection {
        get {
            try { return (GameConnection) Context.Items[GameConnection.ID]!; }
            catch { return null; }
        }
        set { Context.Items[GameConnection.ID] = value; }
    }

    // Locks ------------------------------------------------------------------------------------------------------------------------------
    private readonly Locker Lock = new();

    // Groups -----------------------------------------------------------------------------------------------------------------------------
    private static string GroupName(string code, UPDATE_GROUP group) => $"{code}-{group}";
    private static List<string> GroupNames(GameConnection connection) {
        var code = connection.Engine.Game.Code;
        // 1) Common group:
        List<string> groups = [GroupName(code, UPDATE_GROUP.ALL)];
        // 2) All spectators:
        if (connection.Engine.Game.Display == DISPLAY_MODE.ONE_SCREEN
            && connection.Spectator is Player
            && !connection.Spectator.User.Equals(connection.Engine.Game.Host)
        ) return groups;
        groups.Add(GroupName(code, UPDATE_GROUP.WATCH));
        // 3) Touch spectators:
        if (connection.Spectator.Device != DEVICE_TYPE.TOUCH) return groups;
        groups.Add(GroupName(code, UPDATE_GROUP.WATCH_TOUCH));
        return groups;
    }
    private async Task AddToGroups(GameConnection connection) {
        foreach (var group in GroupNames(connection)) {
            await Groups.AddToGroupAsync(connection.ConnectionID, group);
        }
    }
    private async Task RemoveFromGroups(GameConnection connection) {
        foreach (var group in GroupNames(connection)) {
            await Groups.RemoveFromGroupAsync(connection.ConnectionID, group);
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

    // Connect ----------------------------------------------------------------------------------------------------------------------------
    public override async Task OnConnectedAsync() {
        try { await Lock.Exclusive(async () => {
            // 1) Base call:
            await base.OnConnectedAsync();
            // 2) Read parameters:
            var ctx = Context.GetHttpContext() ?? throw new CoreException(new Error("Missing parameters!"));
                // 2.1) Code:
                if (!ctx.Request.Query.TryGetValue(Game.CODE_ID, out var queryCode))
                    throw new CoreException(new Error("Code not set!"));
                var code = $"{queryCode}";
                // 2.2) User:
                if (!ctx.Request.Query.TryGetValue(User.USER_ID, out var queryUser))
                    throw new CoreException(new Error("User undefined!"));
                var user = JsonSerializer.Deserialize<User>(queryUser!)!;
                // 2.3) Touch device:
                if (!ctx.Request.Query.TryGetValue(Spectator.DEVICE_ID, out var queryDevice))
                    throw new CoreException(new Error("Device parameter not set!"));
                var device = JsonSerializer.Deserialize<DEVICE_TYPE>(queryDevice!)!;
                // 2.4) Watch:
                if (!ctx.Request.Query.TryGetValue(Spectator.WATCH_ID, out var queryWatch))
                    throw new CoreException(new Error("Watch parameter not set!"));
                var watch = bool.Parse(queryWatch!);
            // 3) Try connect to game:
                // 3.1) Create connection:
                var (engine, spectator) = await GameService.Connect(code, user, device, watch);
                GameConnection = new(Context.ConnectionId, engine, spectator);
                // 3.2) Add to groups:
                await AddToGroups(GameConnection);
            // 4) Send response:
            await Clients.Caller.SendAsync(GAME_HUB.CONNECTION_SUCCESSFUL, engine.Game, watch ? null : spectator);
        }); } catch (Exception e) {
            await HandleCallException(e); return;
        }
    }

    // Client updates ---------------------------------------------------------------------------------------------------------------------
    public void KeyUpdate(KeyUpdate update) {
        if (GameConnection is null) return;
        GameService.Update(GameConnection.Engine, update);
    }

    public async Task PingUpdate(PingUpdate update) {
        await Clients.Caller.SendAsync(update.HUB_ACTION, update);
    }

    // Server updates ---------------------------------------------------------------------------------------------------------------------
    public static async Task SendGameUpdate(string code, UPDATE_GROUP group, NetworkUpdate update) {
        try { await Hub.Clients.Group(GroupName(code, group)).SendAsync(update.HUB_ACTION, update); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    public static async Task SendException(string code, UPDATE_GROUP group, CoreException exception) {
        try { await HandleException(Hub.Clients.Group(GroupName(code, group)), exception); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    // Disconnect -------------------------------------------------------------------------------------------------------------------------
    public override async Task OnDisconnectedAsync(Exception? exception) {
        try { await Lock.Exclusive(async () => {
            // 1) Disconnect player from the game:
            if (GameConnection != null) {
                // 1.1) Remove from groups:
                await RemoveFromGroups(GameConnection);
                // 1.2) Disconnect:
                await GameService.Disconnect(GameConnection.Engine, GameConnection.Spectator);
            }
            // 2) Base call:
            await base.OnDisconnectedAsync(exception);
        }); } catch (Exception e) {
            Console.Error.WriteLine(e);
        }
    }
}
