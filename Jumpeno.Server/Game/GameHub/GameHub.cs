namespace Jumpeno.Server.Hubs;

public class GameHub: Hub {
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
        // NOTE: Connection must be closed on the client!
    }

    private async Task HandleCallException(Exception e) {
        await HandleException(Clients.Caller, e);
        Context.Abort();
    }

    // Connect ----------------------------------------------------------------------------------------------------------------------------
    public override async Task OnConnectedAsync() {
        try { await Lock.Lock(async () => {
            // 1) Base call:
            Console.WriteLine($"Connection opened::: {Context.ConnectionId}");
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

            Lock.Unlock();
        }); } catch (Exception e) {
            await HandleCallException(e); return;
        }
    }

    // Client updates ---------------------------------------------------------------------------------------------------------------------

    // Server updates ---------------------------------------------------------------------------------------------------------------------    
    public static async Task SendGameUpdate(string code, GameUpdate update) {
        try { await Hub.Clients.Group(code).SendAsync(update.HUB_ACTION, update); }
        catch (Exception e) { Console.WriteLine(e); }
    }

    public static async Task SendException(string code, GameException exception) {
        try { await HandleException(Hub.Clients.Group(code), exception); }
        catch (Exception e) { Console.WriteLine(e); }
    }

    // Disconnect -------------------------------------------------------------------------------------------------------------------------    
    public override async Task OnDisconnectedAsync(Exception? exception) {
        try { await Lock.Lock(async () => {
            // 1) Disconnect player from game:
            if (GameConnection != null) {
                await Groups.RemoveFromGroupAsync(GameConnection.ConnectionID, GameConnection.Engine.Code);
                await GameService.Disconnect(GameConnection.Engine, GameConnection.Player);
            }

            // 2) Base call:
            Console.WriteLine($"Connection closed::: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);

            Lock.Unlock();
        }); } catch (Exception e) {
            Console.WriteLine(e);
        }
    }
}
