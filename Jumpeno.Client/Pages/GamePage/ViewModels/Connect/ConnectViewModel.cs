namespace Jumpeno.Client.ViewModels;

#pragma warning disable CA1822

public class ConnectViewModel(ConnectViewModelParams @params) {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    public bool Create { get; private set; } = @params.Create;
    public string URLCode => @params.URLCode() ?? "";
    private readonly EventDelegate<GameViewModel> OnConnect = @params.OnConnect ?? EventDelegate<GameViewModel>.EMPTY;
    private readonly EmptyDelegate OnDisconnect = @params.OnDisconnect ?? EmptyDelegate.EMPTY;
    private readonly Action Notify = @params.Notify ?? (() => {});

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Form: (Register for error handling)
    public string? Form { get; private set; } = null;
    public void RegisterForm(string form) => Form = form;
    public void UnregisterForm(string form) { if (Form == form) Form = null; }
    // Authorization:
    private readonly HubAuth Authorization = new();
    // Connection:
    private HubConnection? HubConnection = null;
    private bool IsConnected => HubConnection is not null && HubConnection.State == HubConnectionState.Connected;
    public bool IsConnecting { get; private set; } = false;
    private readonly LockerSlim ConnectLock = new();
    // Game:
    private GameViewModel? GameVM = null;
    private const int FIRST_RENDER_CHECK_INTERVAL = 100; // ms
    // Updates:
    private readonly LinkedList<GameUpdate> PendingUpdates = [];

    // URL Code ---------------------------------------------------------------------------------------------------------------------------
    private string LastURLCode = @params.URLCode() ?? "";
    private event Func<string, Task>? URLCodeChanged;
    private readonly LockerSlim URLCodeChangedLock = new();
    private async Task InvokeURLCodeChanged(bool request = false) {
        await URLCodeChangedLock.TryExclusive(async () => {
            if (
                URLCodeChanged == null || !request && LastURLCode == URLCode ||
                await PageLoader.IsActiveTask(PageLoaderTask.NAVIGATION) ||
                await PageLoader.IsActiveTask(PageLoaderTask.ANIMATION) ||
                await PageLoader.IsActiveTask(PageLoaderTask.GAME_CONNECT)
            ) return;
            await URLCodeChanged.Invoke(URLCode);
            LastURLCode = URLCode;
        });
    }

    // Listeners:
    public async Task AddURLCodeChangedListener(Func<string, Task> listener) {
        await URLCodeChangedLock.TryExclusive(() => URLCodeChanged += listener);
        await InvokeURLCodeChanged(true);
    }

    public async Task RemoveURLCodeChangedListener(Func<string, Task> listener) {
        await URLCodeChangedLock.TryExclusive(() => URLCodeChanged -= listener);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    // NOTE: Lifecycle events [explicitly invoked from page]:
    public async Task OnPageInitializedAsync() => await Task.CompletedTask;

    public async Task OnPageParametersSetAsync() => await InvokeURLCodeChanged();

    public async ValueTask OnPageDisposeAsync() {
        await HandleErrors();
        await ConnectLock.DisposeSafe();
        await URLCodeChangedLock.DisposeSafe();
    }

    // Request actions --------------------------------------------------------------------------------------------------------------------
    // NOTE: Component calls:
    public async Task CreateRequest(CreateData data) => await StartConnection(async () => {
        // 1) Create DTO:
        var dto = new GameHubCreateDTO(
            data.Code, data.GameName,
            data.Map, data.Anonyms, data.Rounds, data.Capacity,
            data.DisplayMode, data.GameMode,
            Token.Access.raw, Window.GetDeviceType()
        );
        // 2) Validation:
        dto.Assert();
        // 3) Connect request:
        await CreateConnection(nameof(GameHubCreateDTO), dto);
    });
    
    public async Task ConnectRequest(ConnectData data) => await StartConnection(async () => {
        if (Auth.IsRegisteredUser) {
            // 1) Create DTO:
            var dto = new GameHubRegisteredDTO(
                data.Code, Token.Access.raw, Window.GetDeviceType(), data.Spectate
            );
            // 2) Validation:
            dto.Assert();
            // 3) Connect request:
            await CreateConnection(nameof(GameHubRegisteredDTO), dto);
        } else {
            // 1) Create DTO:
            var dto = new GameHubAnonymousDTO(
                data.Code, data.Name, Window.GetDeviceType(), data.Spectate
            );
            // 2) Validation:
            dto.Assert();
            // 3) Connect request:
            Auth.LogInAnonymous(data.Name);
            await CreateConnection(nameof(GameHubAnonymousDTO), dto);
        }
    });

    // Connect methods --------------------------------------------------------------------------------------------------------------------
    private async Task StartConnection(Func<Task> request) {
        Navigator.AllowOne();
        await PageLoader.Show(PageLoaderTask.GAME_CONNECT);
        await ConnectLock.TryExclusive(async () => {
            if (!await HTTP.Try(async () => {
                // 1) Authorization:
                Authorization.Start(request);
                // 2) Pending updates:
                PendingUpdates.Clear();
                // 3) Connect request:
                await request();
            }, Form)) {
                Navigator.AllowNone();
                await DisposeGame();
                await HideGameLoaders();
            }
        });
    }

    private async Task CreateConnection<P>(string type, P dto) {
        // 0) Check connection:
        if (IsConnected) throw Exceptions.DEFAULT;
        // 1) Create data URL:
        var q = new QueryParams();
            // 1.1) Add meta:
            q.Set(Header.APP_VERSION, AppSettings.Version);
            // 1.2) Add DTO:
            q.Set(GameHubs.DTO_TYPE, type);
            q.Set(GameHubs.DTO, JsonSerializer.Serialize(dto));
        var hubURL = URL.SetQueryParams(URL.ToAbsolute(GameHubs.URL), q);
        // 2) Create HUB:
        HubConnection = new HubConnectionBuilder().WithUrl(hubURL, options => {
            options.Headers[Header.ACCEPT_LANGUAGE] = I18N.Culture;
        }).Build();
        // 3) Add events:
        HubConnection.On<Game>(GameHubs.CONNECTION_SUCCESSFUL, ConnectionSuccessful);
        HubConnection.On<GameActionResponseUpdate>(GameHubs.GAME_ACTION_RESPONSE_UPDATE, GameResponse);
        HubConnection.On<GameActionResponseUpdate>(GameHubs.PLAYER_KICK_RESPONSE_UPDATE, GameResponse);
        HubConnection.On<GameActionResponseUpdate>(GameHubs.PLAYER_READY_RESPONSE_UPDATE, GameResponse);
        HubConnection.On<RoundUpdate>(GameHubs.ROUND_UPDATE, GameUpdate);
        HubConnection.On<GamePlayUpdate>(GameHubs.GAME_PLAY_UPDATE, GameUpdate);
        HubConnection.On<PlayerUpdate>(GameHubs.PLAYER_UPDATE, GameUpdate);
        HubConnection.On<SpectatorUpdate>(GameHubs.SPECTATOR_UPDATE, GameUpdate);
        HubConnection.On<PingUpdate>(GameHubs.PING_UPDATE, PingUpdate);
        HubConnection.On<AppExceptionDTO>(GameHubs.ERROR, HandleErrors);
        HubConnection.Closed += OnConnectionClosed;
        // 4) Connect:
        await HubConnection.StartAsync();
        // 5) Check connection:
        if (!IsConnected) throw Exceptions.DEFAULT;
    }

    private async Task ConnectionSuccessful(Game game) {
        await ConnectLock.TryExclusive(async () => {
            try {
                // 1) Show loader & check:
                await PageLoader.Show(PageLoaderTask.GAME_CONNECT);
                if (HubConnection is null) throw Exceptions.DEFAULT;
                if (HubConnection.ConnectionId is null) throw Exceptions.DEFAULT;
                IsConnecting = true;
                // 2) Authorization:
                Authorization.OnSuccess();
                // 3) Get player:
                Player? player = game.GetValidPlayerByConnectionID(HubConnection.ConnectionId);
                if (Auth.IsAnonymousUser && player != null) Auth.User.Skin = player.User.Skin;
                // 4) Create ViewModel:
                var qrCode = QRCode.SVG($"{URL.BaseUrl()}{I18N.Link<GamePage>([game.Code])}");
                GameVM = new(
                    qrCode, game, player,
                    PendingUpdates,
                    Send, SendRequest, Exec,
                    @params.Chat, Notify
                );
                PendingUpdates.Clear();
                await GameVM.InitChat();
                await GameVM.AddAfterUpdateListener(GameResponse);
                // 5) Set URL:
                var state = GamePage.NavState.Get();
                bool isCodeSet = URLCode != "";
                if (isCodeSet) await Navigator.NavigateTo(I18N.Link<GamePage>(), replace: true, notify: NotifyType.STATE);
                else await Navigator.SetQueryParams(new());
                GamePage.NavState.Set(new GamePage.HistoryState(state.WasRedirect, Create));
                Navigator.AllowOne();
                await Navigator.NavigateTo(
                    URL.WithQuery(I18N.Link<GamePage>([GameVM.Game.Code]), ""),
                    replace: state.WasRedirect && isCodeSet,
                    state: GamePage.NavState.New(new GamePage.HistoryState(true, Create)),
                    notify: NotifyType.STATE
                );
                // 6) Update and render:
                await OnConnect.Invoke(GameVM); Notify();
                while (true) {
                    if (Page.Current is not GamePage page) { Navigator.Refresh(); throw Exceptions.DEFAULT; }
                    if (GamePage.GAME_VIEWS.Contains(page.View?.GetType())) break;
                    await Task.Delay(FIRST_RENDER_CHECK_INTERVAL);
                }
            } catch {
                // 7) Handle errors:
                Navigator.AllowNone();
                IsConnecting = false;
                await DisposeGame();
                Notification.Error(Messages.DEFAULT.T);
            } finally {
                // 8) Finalize:
                IsConnecting = false;
                await HideGameLoaders();
            }
        });
    }

    // Client actions ---------------------------------------------------------------------------------------------------------------------
    private async Task Send(string action, object message) {
        await ConnectLock.TryExclusive(async () => {
            if (HubConnection is null) return;
            await HubConnection.SendAsync(action, message);
        });
    }

    private async Task SendRequest(string action, object message) {
        await ConnectLock.TryExclusive(async () => {
            if (HubConnection is null) return;
            await PageLoader.Show(PageLoaderTask.GAME_REQUEST);
            await HubConnection.SendAsync(action, message);
        });
    }

    private async Task Exec(EmptyDelegate action) {
        await ConnectLock.TryExclusive(async () => {
            if (HubConnection is null) return;
            await action.Invoke();
        });
    }

    // Server actions ---------------------------------------------------------------------------------------------------------------------
    private async Task GameUpdate(GameUpdate update) {
        // 1) Determ ViewModel:
        GameViewModel? vm = null;
        // 2.1) Save pending:
        if (await ConnectLock.TryExclusive(() => {
            if (GameVM != null) { vm = GameVM; return false; }
            PendingUpdates.AddLast(update); return true;
        }, true)) return;
        // 2.2) Or update game:
        if (vm != null) await vm.AddUpdate(update);
    }

    private void PingUpdate(PingUpdate update) => GameVM?.SetPing(update);

    private async Task GameResponse(GameResponseUpdate update) {
        await ConnectLock.TryExclusive(async () => {
            // 1) Check connection:
            if (HubConnection == null) return;
            // 2) Show error if any: 
            if (update.Exception is AppExceptionDTO e) ErrorHandler.Display(update.Exception.Exception);
            // 3) Hide loader after request:
            await PageLoader.Hide(PageLoaderTask.GAME_REQUEST);
        });
    }

    private async Task GameResponse(UpdateAfterEvent e) {
        // 1) Check network update:
        if (e.Update is not NetworkUpdate netUpdate) return;
        // 2) Check response:
        if (netUpdate.ResponseIDs == null) return;
        // 3) React to response:
        await ConnectLock.TryExclusive(async () => {
            // 3.1) Check connection:
            if (HubConnection?.ConnectionId == null) return;
            // 3.2) Check if for me:
            if (!netUpdate.ResponseIDs.Contains(HubConnection.ConnectionId)) return;
            // 3.3) Hide loader after application:
            await PageLoader.Hide(PageLoaderTask.GAME_REQUEST);
        });
    }

    // Error handling ---------------------------------------------------------------------------------------------------------------------
    private async Task HandleErrors(AppExceptionDTO? exceptionDTO = null) {
        await ConnectLock.TryExclusive(async () => {
            if (HubConnection == null) return;
            Navigator.AllowNone();
            await PageLoader.Show(PageLoaderTask.GAME_CONNECT);
            if (exceptionDTO is null) {
                await DisposeGame();
            } else {
                // 1) Authorization:
                var exception = await Authorization.OnError(exceptionDTO, DisposeGame) ?? exceptionDTO.Exception;
                // 2) Disconnect:
                if (exception.Code == Codes.DISCONNECT) await DisposeGame();
                // 3) Display errors:
                ErrorHandler.Display(exception, Form);
            }
            await HideGameLoaders();
        });
    }

    private async Task OnConnectionClosed(Exception? e) {
        if (e is not null) await HandleErrors(Exceptions.DISCONNECT.DTO);
    }

    // Disposal ---------------------------------------------------------------------------------------------------------------------------
    private async Task DisposeHub() {
        if (HubConnection is not null) {
            await HubConnection.DisposeAsync();
            HubConnection = null;
        }
        IsConnecting = false;
    }

    private async Task DisposeGame() {
        await DisposeHub();
        var wasConnected = GameVM != null;
        if (GameVM != null) {
            await GameVM.RemoveAfterUpdateListener(GameResponse);
            await GameVM.DisposeAsync();
            GameVM = null;
        }
        PendingUpdates.Clear();
        if (Auth.IsAnonymousUser) Auth.LogOutAnonymous();
        if (wasConnected) await OnDisconnect.Invoke();
    }

    private async Task HideGameLoaders() {
        await PageLoader.Hide(PageLoaderTask.GAME_CONNECT);
        await PageLoader.Hide(PageLoaderTask.GAME_REQUEST);
        Navigator.AllowAny();
        Notify();
    }
}
