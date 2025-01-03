namespace Jumpeno.Client.ViewModels;

public class ConnectViewModel(ConnectViewModelParams @params) {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    private string URLCode => @params.URLCode() ?? "";
    private MainLayoutViewModel? LayoutVM => @params.LayoutVM();
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
    // History:
    private record GamePageHistoryState(bool WasRedirect);
    // Game:
    private GameViewModel? GameVM = null;
    private TaskCompletionSource? GameViewTCS = null;
    private void GameViewRendered() => GameViewTCS?.TrySetResult();
    // Updates:
    private readonly LinkedList<GameUpdate> PendingUpdates = [];

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    // NOTE: Lifecycle events must be explicitly invoked in page:
    public async Task OnPageInitializedAsync() {
        await ConnectLock.Exclusive(async () => {
            await Navigator.AddAfterEventListener(OnPageLeave);
        });
    }

    public async Task OnPageParametersSetAsync() => await InvokeURLCodeChanged();

    public async ValueTask OnPageDisposeAsync() {
        await ConnectLock.Exclusive(async () => {
            await DisposeGame();
            await Navigator.RemoveAfterEventListener(OnPageLeave);
        });
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    // URLCode change event:
    private string LastURLCode = @params.URLCode() ?? "";
    private event Func<string, Task>? URLCodeChanged;
    private readonly LockerSlim URLCodeChangedLock = new();
    private async Task InvokeURLCodeChanged(bool request = false) {
        if (IsConnecting) return;
        await URLCodeChangedLock.Exclusive(async () => {
            if (URLCodeChanged == null || (!request && LastURLCode == URLCode)) return;
            await URLCodeChanged.Invoke(URLCode);
            LastURLCode = URLCode;
        });
    }
    // NOTE: Add listeners in components:
    public async Task AddURLCodeChangedListener(Func<string, Task> listener) {
        await URLCodeChangedLock.Exclusive(() => URLCodeChanged += listener);
        await InvokeURLCodeChanged(true);
    }

    public async Task RemoveURLCodeChangedListener(Func<string, Task> listener) {
        await URLCodeChangedLock.Exclusive(() => URLCodeChanged -= listener);
    }

    // Request actions --------------------------------------------------------------------------------------------------------------------
    // NOTE: Call actions in components:
    public async Task ConnectRequest(ConnectData data, bool spectator) {
        try {
            await ConnectLock.Lock();
            await PageLoader.Show(PAGE_LOADER_TASK.GAME);
            PendingUpdates.Clear();
            if (!Auth.IsRegistered()) Auth.LogInAnonymous(data.Name, User.GenerateSkin());
            if (!await CreateConnection(data.Code, spectator)) throw new CoreException();
        } catch {
            Notification.Error(I18N.T("Something went wrong."));
            await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
        } finally {
            ConnectLock.Unlock();
        }
    }

    // Connect methods --------------------------------------------------------------------------------------------------------------------
    private async Task<bool> CreateConnection(string code, bool spectator) {
        if (IsConnected) return true;
        try {
            // 1) Create data URL:
            var q = new QueryParams();
            q.Set(Game.CODE_ID, code);
            q.Set(User.USER_ID, JsonSerializer.Serialize(Auth.User));
            q.Set(Connection.DEVICE_ID, JsonSerializer.Serialize(Navigator.IsTouchDevice ? DEVICE_TYPE.TOUCH : DEVICE_TYPE.POINTER));
            q.Set(Spectator.SPECTATOR_ID, spectator);
            var hubURL = URL.SetQueryParams(URL.ToAbsolute(GAME_HUB.ROUTE_CULTURE()), q);
            // 2) Create HUB:
            HubConnection = new HubConnectionBuilder().WithUrl(hubURL).Build();
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
        } catch (Exception) {
            await DisposeHub();
        }
        return IsConnected;
    }

    private async Task ConnectionSuccessful(Game game, Player? player) {
        try {
            await ConnectLock.Lock();
            await PageLoader.Show(PAGE_LOADER_TASK.GAME);
            IsConnecting = true;
    
            // 1) Create ViewModel:
            GameVM = new(game, player, PendingUpdates, Send, new(GameViewRendered));
            PendingUpdates.Clear();
            await GameVM.AddNotifyListener(NotifyListener);

            // 2) Set URL:
            var state = Navigator.State<GamePageHistoryState>();
            bool isCodeSet = URLCode != "";
            if (isCodeSet) await Navigator.NavigateTo(I18N.Link<GamePage>([""]), replace: true);
            await Navigator.NavigateTo(
                I18N.Link<GamePage>([GameVM.Game.Code]),
                replace: state.WasRedirect && isCodeSet
            );
            Navigator.SetState<GamePageHistoryState>(new(true));

            // 3) Update and render:
            GameViewTCS = new TaskCompletionSource();
            await OnConnect.Invoke(GameVM);
            await Notify.Invoke();
            await GameViewTCS.Task;

            // 4) Full screen:
            LayoutVM?.HideNavigation(false);
            ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
        } catch {
            GameVM = null;
            Notification.Error(I18N.T("Something went wrong."));
        } finally {
            IsConnecting = false;
            await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
            ConnectLock.Unlock();
        }
    }

    // Client actions ---------------------------------------------------------------------------------------------------------------------
    private async Task Send(string action, object message) {
        await ConnectLock.Exclusive(async () => {
            if (HubConnection is null) return;
            await HubConnection.SendAsync(action, message);
        });
    }

    // Server actions ---------------------------------------------------------------------------------------------------------------------
    private async Task GameUpdate(GameUpdate update) {
        await ConnectLock.Exclusive(async () => {
            if (update is PingUpdate ping) ping.SetReturn();
            if (GameVM is GameViewModel VM) await VM.AddUpdate(update);
            else PendingUpdates.AddLast(update);
        });
    }

    // Error handling ---------------------------------------------------------------------------------------------------------------------
    private async Task HandleErrors(CoreExceptionDTO? exception = null) {
        await ConnectLock.Exclusive(async () => {
            if (exception is null) await DisposeGame();
            else {
                if (exception.Type == EXCEPTION_TYPE.EXCEPTION) await DisposeGame();
                ErrorHandler.NotifyErrors(exception, fallback: true);
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
        var pagePath = I18N.Link<GamePage>([""]);
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
        if (GameVM != null) await GameVM.RemoveNotifyListener(NotifyListener);
        GameVM = null;
        GameViewTCS = null;
        PendingUpdates.Clear();
        if (!Auth.IsRegistered()) Auth.LogOut();
        await OnDisconnect.Invoke();
        LayoutVM?.ShowNavigation();
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }
}
