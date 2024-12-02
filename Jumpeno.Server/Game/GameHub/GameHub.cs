namespace Jumpeno.Server.Hubs;

public class GameHub : Hub {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static IHubContext<GameHub> Hub => AppEnvironment.GetService<IHubContext<GameHub>>();
    private GameConnection? GameConnection {
        get { try { return (GameConnection) Context.Items[GameConnection.ID]!; } catch { return null; } }
        set { Context.Items[GameConnection.ID] = value; }
    }

    // Locks ------------------------------------------------------------------------------------------------------------------------------
    private readonly Locker Lock = new();

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(WebApplication app) {
        app.MapHub<GameHub>(GAME_HUB.ROUTE_EN);
        app.MapHub<GameHub>(GAME_HUB.ROUTE_SK);
    }

    // Exceptions -------------------------------------------------------------------------------------------------------------------------
    private static async Task HandleException(IClientProxy proxy, Exception e) {
        await proxy.SendAsync(GAME_HUB.ERROR, (e is GameException exception ? exception : new GameException()).DataTransferObject());
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
            string code;
            User user;
            var ctx = Context.GetHttpContext() ?? throw new GameException();
            code = $"{ctx.Request.Query[Game.CODE_ID]}";
            user = JsonSerializer.Deserialize<User>(ctx.Request.Query[User.USER_ID]!) ?? throw new GameException();
            // 3) Try connect to game:
            var enginePlayer = await GameService.Connect(code, user);
            GameConnection = new(Context.ConnectionId, enginePlayer.Engine, enginePlayer.Player);
            await Groups.AddToGroupAsync(GameConnection.ConnectionID, GameConnection.Engine.Code);
            await Clients.Caller.SendAsync(GAME_HUB.CONNECTION_SUCCESSFUL, GameConnection.Engine.ClientGameCopy(), GameConnection.Player);
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
    public static async Task SendGameUpdate(string code, NetworkUpdate update) {
        try { await Hub.Clients.Group(code).SendAsync(update.HUB_ACTION, update); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    public static async Task SendException(string code, GameException exception) {
        try { await HandleException(Hub.Clients.Group(code), exception); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    // Disconnect -------------------------------------------------------------------------------------------------------------------------    
    public override async Task OnDisconnectedAsync(Exception? exception) {
        try { await Lock.Exclusive(async () => {
            // 1) Disconnect player from the game:
            if (GameConnection != null) {
                await Groups.RemoveFromGroupAsync(GameConnection.ConnectionID, GameConnection.Engine.Code);
                await GameService.Disconnect(GameConnection.Engine, GameConnection.Player);
            }
            // 2) Base call:
            await base.OnDisconnectedAsync(exception);
        }); } catch (Exception e) {
            Console.Error.WriteLine(e);
        }
    }
}
