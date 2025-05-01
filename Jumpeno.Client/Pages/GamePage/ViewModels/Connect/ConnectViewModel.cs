namespace Jumpeno.Client.ViewModels;

public class ConnectViewModel(ConnectViewModelParams @params) {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string WATCH_QUERY = "watch";
    public const string PRESENT_QUERY = "present";

    // Auto-Watch & Presentation ----------------------------------------------------------------------------------------------------------
    public static bool IsAutoWatch => URL.GetQueryParams().IsTrue(WATCH_QUERY);
    public static bool IsPresentation => URL.GetQueryParams().IsTrue(PRESENT_QUERY);
    public static bool RunAutoWatch => IsAutoWatch && CookieStorage.CookiesAccepted;
    public static bool RunPresentation => IsPresentation && CookieStorage.CookiesAccepted;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    public bool Create { get; private set; } = @params.Create;
    private string URLCode => @params.URLCode() ?? "";
    private readonly EventDelegate<GameViewModel> OnConnect = @params.OnConnect ?? EventDelegate<GameViewModel>.EMPTY;
    private readonly EmptyDelegate OnDisconnect = @params.OnDisconnect ?? EmptyDelegate.EMPTY;
    private readonly EmptyDelegate Notify = @params.Notify ?? EmptyDelegate.EMPTY;
    private readonly Func<Task> NotifyListener = async () => await (@params.Notify ?? EmptyDelegate.EMPTY).Invoke();

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Connection:
    private HubConnection? HubConnection = null;
    private bool IsConnected => HubConnection is not null && HubConnection.State == HubConnectionState.Connected;
    private bool IsConnecting = false;
    private readonly LockerSlim ConnectLock = new();
    // Game:
    private GameViewModel? GameVM = null;
    private TaskCompletionSource? GameViewTCS = new();
    private void GameViewRendered() => GameViewTCS?.TrySetResult();
    // Updates:
    private readonly LinkedList<GameUpdate> PendingUpdates = [];
    private bool Updated = false;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    // NOTE: Lifecycle events must be explicitly invoked in page:
    public async Task OnPageInitializedAsync() {
        await ConnectLock.TryExclusive(async () => {
            await Navigator.AddAfterEventListener(OnPageLeave);
        });
    }

    public async Task OnPageParametersSetAsync() => await InvokeURLCodeChanged();

    public async ValueTask OnPageDisposeAsync() {
        await ConnectLock.TryExclusive(async () => {
            await DisposeGame();
            await Navigator.RemoveAfterEventListener(OnPageLeave);
        });
        ConnectLock.Dispose();
        URLCodeChangedLock.Dispose();
        GC.SuppressFinalize(this);
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    // URLCode change event:
    private string LastURLCode = @params.URLCode() ?? "";
    private event Func<string, Task>? URLCodeChanged;
    private readonly LockerSlim URLCodeChangedLock = new();
    private async Task InvokeURLCodeChanged(bool request = false) {
        if (IsConnecting || await PageLoader.IsActiveTask(PAGE_LOADER_TASK.ANIMATION)) return;
        await URLCodeChangedLock.TryExclusive(async () => {
            if (URLCodeChanged == null || (!request && LastURLCode == URLCode)) return;
            await URLCodeChanged.Invoke(URLCode);
            LastURLCode = URLCode;
        });
    }
    // NOTE: Add listeners in components:
    public async Task AddURLCodeChangedListener(Func<string, Task> listener) {
        await URLCodeChangedLock.TryExclusive(() => URLCodeChanged += listener);
        await InvokeURLCodeChanged(true);
    }

    public async Task RemoveURLCodeChangedListener(Func<string, Task> listener) {
        await URLCodeChangedLock.TryExclusive(() => URLCodeChanged -= listener);
    }

    // Request actions --------------------------------------------------------------------------------------------------------------------
    // NOTE: Call actions in components:
    public async Task ConnectRequest(ConnectData data, bool spectator) {
        await ConnectLock.TryExclusive(async () => {
            try {
                await PageLoader.Show(PAGE_LOADER_TASK.GAME);
                PendingUpdates.Clear();
                Updated = false;
                if (!Auth.IsRegisteredUser) Auth.LogInAnonymous(data.Name, User.GenerateSkin());
                if (!await CreateConnection(data.Code, spectator)) throw new CoreException();
            } catch {
                Notification.Error(I18N.T("Something went wrong."));
                await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
            }
        });
    }

    // Connect methods --------------------------------------------------------------------------------------------------------------------
    private async Task<bool> CreateConnection(string code, bool spectator) {
        if (IsConnected) return true;
        try {
            // 1) Create data URL:
            var q = new QueryParams();
            q.Set(GameValidator.CODE, code);
            q.Set(nameof(User), JsonSerializer.Serialize(Auth.User));
            q.Set(nameof(Connection.Device), JsonSerializer.Serialize(Window.IsTouchDevice ? DEVICE_TYPE.TOUCH : DEVICE_TYPE.POINTER));
            q.Set(nameof(Spectator), spectator);
            var hubURL = URL.SetQueryParams(URL.ToAbsolute(GAME_HUB.URL), q);
            // 2) Create HUB:
            HubConnection = new HubConnectionBuilder().WithUrl(hubURL, options => {
                try { options.Headers[HEADER.AUTHORIZATION] = Token.Access.raw; } catch {}
                options.Headers[HEADER.ACCEPT_LANGUAGE] = I18N.Culture;
            }).Build();
            // 3) Add events:
            HubConnection.On<Game, Player?>(GAME_HUB.CONNECTION_SUCCESSFUL, ConnectionSuccessful);
            HubConnection.On<RoundUpdate>(GAME_HUB.ROUND_UPDATE, GameUpdate);
            HubConnection.On<GamePlayUpdate>(GAME_HUB.GAME_PLAY_UPDATE, GameUpdate);
            HubConnection.On<PlayerUpdate>(GAME_HUB.PLAYER_UPDATE, GameUpdate);
            HubConnection.On<SpectatorUpdate>(GAME_HUB.SPECTATOR_UPDATE, GameUpdate);
            HubConnection.On<PingUpdate>(GAME_HUB.PING_UPDATE, GameUpdate);
            HubConnection.On<CoreExceptionDTO>(GAME_HUB.ERROR, HandleErrors);
            HubConnection.Closed += OnConnectionClosed;
            // 4) Connect:
            await HubConnection.StartAsync();
        } catch {
            await DisposeHub();
        }
        return IsConnected;
    }

    private async Task ConnectionSuccessful(Game game, Player? player) {
        await ConnectLock.TryExclusive(async () => {
            try {
                await PageLoader.Show(PAGE_LOADER_TASK.GAME);
                IsConnecting = true;
        
                // 1) Create ViewModel:
                var qrCode = QRCode.SVG($"{URL.BaseUrl()}{I18N.Link<GamePage>([game.Code])}");
                GameVM = new(qrCode, game, player, PendingUpdates, Send, new(GameViewRendered));
                PendingUpdates.Clear();
                await GameVM.AddNotifyListener(NotifyListener);
                await GameVM.PreRender();

                // 2) Set URL:
                var state = Navigator.State(GamePage.DEFAULT_HISTORY_STATE);
                bool isCodeSet = URLCode != "";
                var query = !GameVM.IsPlayer && IsPresentation ? $"?{PRESENT_QUERY}=true" : "";
                if (isCodeSet) await Navigator.NavigateTo(I18N.Link<GamePage>(), replace: true, notify: NOTIFY.STATE);
                else await Navigator.SetQueryParams(new());
                Navigator.SetState(new GamePage.HistoryState(state.WasRedirect, Create));
                await Navigator.NavigateTo(
                    URL.WithQuery(I18N.Link<GamePage>([GameVM.Game.Code]), query),
                    replace: state.WasRedirect && isCodeSet,
                    state: new GamePage.HistoryState(true, Create),
                    notify: NOTIFY.STATE
                );

                // 3) Update and render:
                GameViewTCS = new TaskCompletionSource();
                await OnConnect.Invoke(GameVM);
                await Notify.Invoke();
                await GameViewTCS.Task;
            } catch {
                GameVM = null;
                Notification.Error(I18N.T("Something went wrong."));
            } finally {
                IsConnecting = false;
                await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
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

    // Server actions ---------------------------------------------------------------------------------------------------------------------    
    private async Task GameUpdate(GameUpdate update) {
        if (Updated) await HandleUpdate(update);
        else await ConnectLock.TryExclusive(async () => await HandleUpdate(update));
    }

    private async Task HandleUpdate(GameUpdate update) {
        if (update is PingUpdate ping) ping.SetReturn();
        if (GameVM is GameViewModel VM) { await VM.AddUpdate(update); Updated = true; }
        else PendingUpdates.AddLast(update);
    }

    // Error handling ---------------------------------------------------------------------------------------------------------------------
    private async Task HandleErrors(CoreExceptionDTO? exception = null) {
        await ConnectLock.TryExclusive(async () => {
            if (exception is null) await DisposeGame();
            else {
                if (exception.Type == EXCEPTION_TYPE.EXCEPTION) await DisposeGame();
                ErrorHandler.NotifyErrors(exception, fallback: true, translate: true);
                ErrorHandler.MarkInputs(exception, translate: true);
            }
            await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
        });
    }

    private async Task OnConnectionClosed(Exception? e) {
        if (e is not null) await HandleErrors(
            new CoreException(new Error("You have been disconnected from the server.")).DTO
        );
    }

    private async Task OnPageLeave(NavigationEvent e) {
        var pagePath = I18N.Link<GamePage>();
        if (!((URL.Path(e.BeforeURL) != pagePath) && (URL.Path(e.AfterURL) == pagePath)) || IsConnecting) {
            return;
        }
        await HandleErrors();
    }

    // Dispose methods --------------------------------------------------------------------------------------------------------------------
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
        if (GameVM != null) await GameVM.RemoveNotifyListener(NotifyListener);
        GameVM?.Dispose();
        GameVM = null;
        GameViewTCS = null;
        PendingUpdates.Clear();
        Updated = false;
        if (Auth.IsAnonymousUser) Auth.LogOutAnonymous();
        if (wasConnected) await OnDisconnect.Invoke();
    }
}
