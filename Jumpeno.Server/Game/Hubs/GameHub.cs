namespace Jumpeno.Server.Hubs;

using Microsoft.AspNetCore.SignalR;

public class GameHub: Hub {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(WebApplication app) {
        app.MapHub<GameHub>(GAME_HUB.ROUTE_EN);
        app.MapHub<GameHub>(GAME_HUB.ROUTE_SK);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public override async Task OnConnectedAsync() {
        await base.OnConnectedAsync();
    }

    public async Task ConnectToGameRequest(string code, User user) {
        await Clients.Caller.SendAsync(GAME_HUB.CONNECT_TO_GAME_RESPONSE, new Game(), new List<string>());
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        await base.OnDisconnectedAsync(exception);
    }
}
