namespace Jumpeno.Server.Hubs;

public class GameHub: Hub {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static IHubContext<GameHub> Hub => AppEnvironment.GetService<IHubContext<GameHub>>();
    private GameConnection? GameConnection {
        get { try { return (GameConnection) Context.Items[GameConnection.ID]!; } catch { return null; } }
        set { Context.Items[GameConnection.ID] = value; }
    }

    // Locks ------------------------------------------------------------------------------------------------------------------------------
    private readonly Semaphore Lock = new(1, 1);

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
        // 1) Base call:
        await base.OnConnectedAsync();
        
        // 2) Read parameters:
        string code;
        User user;
        try {
            var ctx = Context.GetHttpContext() ?? throw new GameException();
            code = $"{ctx.Request.Query[Game.CODE_ID]}";
            user = JsonSerializer.Deserialize<User>(ctx.Request.Query[User.USER_ID]!) ?? throw new GameException();
        } catch (Exception e) {
            await HandleCallException(e); return;
        }

        // 3) Try connect to game:
        try {
            Lock.WaitOne();
            GameConnection = await GameService.Connect(Context.ConnectionId, code, user);
            await Groups.AddToGroupAsync(GameConnection.Player.ConnectionID, GameConnection.GameEngine.Code);
            await Clients.Caller.SendAsync(GAME_HUB.CONNECTION_SUCCESSFUL, GameConnection.GameEngine.ClientGameCopy(), GameConnection.Player);
        } catch (Exception e) {
            await HandleCallException(e); return;
        } finally {
            Lock.Release();
        }
    }

    // Client updates ---------------------------------------------------------------------------------------------------------------------

    // Server updates ---------------------------------------------------------------------------------------------------------------------
    public static async Task GamePlayerUpdate(GameEngine engine, Player player, PLAYER_ACTION action) {
        await Hub.Clients.Group(engine.Code).SendAsync(
            GAME_HUB.GAME_PLAYER_UPDATE, new GamePlayerUpdate(engine.Time, engine.State, player, action)
        );
    }

    // Disconnect -------------------------------------------------------------------------------------------------------------------------    
    public override async Task OnDisconnectedAsync(Exception? exception) {
        // 1) Disconnect player from game:
        if (GameConnection != null) {
            try {
                Lock.WaitOne();
                await Groups.RemoveFromGroupAsync(GameConnection.Player.ConnectionID, GameConnection.GameEngine.Code);
                await GameService.Disconnect(GameConnection);
            } finally {
                Lock.Release();
            }
        }

        // 2) Base call:
        await base.OnDisconnectedAsync(exception);
    }
}
