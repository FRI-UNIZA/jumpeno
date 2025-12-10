namespace Jumpeno.Server.Hubs;

#pragma warning disable CS1998

public class GameHub : Hub {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(WebApplication app) => app.MapHub<GameHub>(GAME_HUB.URL);

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
    private static string GroupName(ulong id, string code, UPDATE_GROUP group) => $"{id}-{code}-{group}";
    private static List<string> GroupNames(GameContext ctx) {
        var id = ctx.Engine.Game.ID;
        var code = ctx.Engine.Game.Code;
        // 1) Common group:
        List<string> groups = [GroupName(id, code, UPDATE_GROUP.ALL)];
        // 2) All spectators:
        if (
            ctx.Engine.Game.DisplayMode != DISPLAY_MODE.EACH_OWN
            && ctx.Connection is Player
            && !ctx.Connection.User.Equals(ctx.Engine.Game.Host)
        ) return groups;
        groups.Add(GroupName(id, code, UPDATE_GROUP.WATCH));
        // 3) Touch spectators:
        if (ctx.Connection.Device != DEVICE_TYPE.TOUCH) return groups;
        groups.Add(GroupName(id, code, UPDATE_GROUP.WATCH_TOUCH));
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
        await proxy.SendAsync(GAME_HUB.ERROR, (e is AppException exception ? exception : EXCEPTION.DEFAULT).DTO);
        // NOTE: Client must close the connection!
    }

    private async Task HandleCallException(Exception e) {
        await HandleException(Clients.Caller, e);
        // NOTE: Automatically closes connection:
        Context.Abort();
    }

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    private async Task<(User User, object DTO)> ReadDTO() {
        // 1.1) Init context:
        var ctx = Context.GetHttpContext() ?? throw EXCEPTION.SERVER;
        // 1.2) Check app version:
        VersionMiddleware.CheckHubVersion(ctx);
        // 1.3) Init errors:
        List<Error> errors = [];

        // 2.1) Validate type:
        Checker.Validate(
            errors,
            !ctx.Request.Query.TryGetValue(GAME_HUB.DTO_TYPE, out var queryDTOType),
            ERROR.EMPTY.SetID(GAME_HUB.DTO_TYPE)
        );
        // 2.2) Validate params:
        Checker.Validate(
            errors,
            !ctx.Request.Query.TryGetValue(GAME_HUB.DTO, out var queryDTO),
            ERROR.EMPTY.SetID(GAME_HUB.DTO)
        );
        // 2.3) Check errors:
        Checker.AssertWith(errors, EXCEPTION.VALUES);

        // 3.1) Read params:
        switch (queryDTOType) {
            case nameof(GameHubCreateDTO): {
                var dto = JsonSerializer.Deserialize<GameHubCreateDTO>(queryDTO!)
                ?? throw EXCEPTION.VALUES.SetErrors(ERROR.UNDEFINED.SetID(GAME_HUB.DTO));
                dto.Assert();
                JWT.Authorize(dto.AccessToken, [ROLE.USER]);
                return (await UserEntity.SelectCurrentActivatedUser(), dto);
            }
            case nameof(GameHubAnonymousDTO): {
                var dto = JsonSerializer.Deserialize<GameHubAnonymousDTO>(queryDTO!)
                ?? throw EXCEPTION.VALUES.SetErrors(ERROR.UNDEFINED.SetID(GAME_HUB.DTO));
                dto.Assert();
                return (new(dto.Name), dto);
            }
            case nameof(GameHubRegisteredDTO): {
                var dto = JsonSerializer.Deserialize<GameHubRegisteredDTO>(queryDTO!)
                ?? throw EXCEPTION.VALUES.SetErrors(ERROR.UNDEFINED.SetID(GAME_HUB.DTO));
                dto.Assert();
                JWT.Authorize(dto.AccessToken, [ROLE.USER]);
                return (await UserEntity.SelectCurrentActivatedUser(), dto);
            }
        }
        // 3.2) Invalid type:
        throw EXCEPTION.VALUES.SetErrors(ERROR.INVALID.SetID(GAME_HUB.DTO_TYPE));
    }

    // Connect ----------------------------------------------------------------------------------------------------------------------------
    public static async Task BeforeConnected() {}
    public override async Task OnConnectedAsync() {
        try {
            // 1) Create TCS:
            ConnectTCS = new();
            // 2) Read DTO from query params:
            var (user, dto) = await ReadDTO();
            // 3) Connect to or create game:
            // NOTE: [Locked] BeforeConnected()
            GameContext = dto switch {
                GameHubCreateDTO data =>
                    await GameService.Create(data, new(Context.ConnectionId, user, data.Device)),
                GameHubAnonymousDTO data =>
                    await GameService.Connect(data.Code, new(Context.ConnectionId, user, data.Device), data.Spectate, nameof(data.Code), nameof(data.Name)),
                GameHubRegisteredDTO data =>
                    await GameService.Connect(data.Code, new(Context.ConnectionId, user, data.Device), data.Spectate, nameof(data.Code)),
                _ => throw EXCEPTION.VALUES.SetErrors(ERROR.INVALID.SetID(GAME_HUB.DTO)),
            };
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
        await Hub.Clients.Client(id).SendAsync(GAME_HUB.CONNECTION_SUCCESSFUL, ctx.Engine.Game);
    }

    // Client updates ---------------------------------------------------------------------------------------------------------------------
    public async Task GameActionRequestUpdate(GameActionRequestUpdate update) {
        try {
            // 1) Validate host:
            if (GameContext is null || GameContext.Connection.User.ID != GameContext.Engine.Game.Host.ID)
                throw EXCEPTION.CLIENT.SetInfo("You are not a host!");
            // 2) Control game:
            switch (update.Action) {
                case GAME_ACTION.START: await GameService.StartGame(GameContext); return;
                case GAME_ACTION.PAUSE: await GameService.PauseGame(GameContext); return;
                case GAME_ACTION.TOGGLE: await GameService.ToggleGame(GameContext); return;
                case GAME_ACTION.DELETE: await GameService.DeleteGame(GameContext); return;
            }
            // 3) Throw if invalid:
            throw EXCEPTION.CLIENT.SetInfo("Invalid game action!");
        } catch (Exception e) {
            // 4) Handle error:
            await SendResponse(new GameActionResponseUpdate(e));
        }
    }

    public async Task PlayerReadyRequestUpdate(PlayerReadyRequestUpdate update) {
        try {
            // 1) Validate player:
            if (GameContext == null) throw EXCEPTION.CLIENT.SetInfo("You are not a player!");
            // 2) Set player ready:
            await GameService.SetPlayerReady(GameContext);
        } catch (Exception e) {
            // 3) Handle error:
            await SendResponse(new PlayerReadyResponseUpdate(e));
        }
    }

    public async Task PlayerKickRequestUpdate(PlayerKickRequestUpdate update) {
        try {
            // 1) Validate host:
            if (GameContext is null || GameContext.Connection.User.ID != GameContext.Engine.Game.Host.ID)
                throw EXCEPTION.CLIENT.SetInfo("You are not a host!");
            // 2) Kick player:
            await GameService.KickPlayerByName(GameContext, update.Name);
        } catch (Exception e) {
            // 3) Handle error:
            await SendResponse(new PlayerKickResponseUpdate(e));
        }
    }

    public void KeyUpdate(KeyUpdate update) {
        try {
            // 1) Validate player:
            if (GameContext?.Connection is not Player player) return;
            if (player.ID != update.PlayerID) return;
            // 2) Update game:
            GameService.Update(GameContext, update);
        } catch (Exception e) {
            // 3) Handle error:
            Console.Error.WriteLine(e);
        }
    }

    public async Task PingUpdate(PingUpdate update) {
        try { await Clients.Caller.SendAsync(update.HUB_ACTION, update); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    // Server updates ---------------------------------------------------------------------------------------------------------------------
    public static async Task SendGameUpdate(Game game, UPDATE_GROUP group, NetworkUpdate update) {
        try { await Hub.Clients.Group(GroupName(game.ID, game.Code, group)).SendAsync(update.HUB_ACTION, update); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    public static async Task SendGameUpdate(Connection? connection, NetworkUpdate update) {
        try {
            if (connection == null || connection.ConnectionID is not string id) return;
            await Hub.Clients.Client(id).SendAsync(update.HUB_ACTION, update);
        } catch (Exception e) {
            Console.Error.WriteLine(e);
        }
    }

    public static async Task SendException(Game game, UPDATE_GROUP group, AppException exception) {
        try { await HandleException(Hub.Clients.Group(GroupName(game.ID, game.Code, group)), exception); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    public static async Task SendException(Connection? connection, AppException exception) {
        try {
            if (connection == null || connection.ConnectionID is not string id) return;
            await HandleException(Hub.Clients.Client(id), exception);
        } catch (Exception e) {
            Console.Error.WriteLine(e);
        }
    }

    public async Task SendResponse(GameResponseUpdate update) {
        try { await Clients.Caller.SendAsync(update.HUB_ACTION, update); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    public static async Task SendResponse(Connection? connection, GameResponseUpdate update) {
        try {
            if (connection == null || connection.ConnectionID is not string id) return;
            await Hub.Clients.Client(id).SendAsync(update.HUB_ACTION, update);
        } catch (Exception e) {
            Console.Error.WriteLine(e);
        }
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
            // NOTE: Not called for invalid (already kicked) player - exception is thrown!
            // 3) Disconnect player from the game:
            // NOTE: [Locked] BeforeDisconnected(GameContext)
            await GameService.Disconnect(GameContext);
            // NOTE: [Locked] AfterDisconnected(GameContext)
        } catch (Exception e) {
            // 4) Handle error:
            Console.Error.WriteLine(e);
        }
    }
    public static async Task AfterDisconnected(GameContext ctx) {}
}
