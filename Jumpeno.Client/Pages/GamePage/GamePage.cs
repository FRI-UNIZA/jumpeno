namespace Jumpeno.Client.Pages;

public partial class GamePage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ROUTE_EN = "/en/game/{URLCode?}";
    public const string ROUTE_SK = "/sk/hra/{URLCode?}";
    public static string Link(string url, string code) {
        return URL.ReplaceSegments(url, new() {{ 1, $"{code}" }});
    }

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public required MainLayoutViewModel LayoutVM { get; set; }
    [Parameter]
    public string? URLCode { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string? Code = null;
    private TaskCompletionSource? GameViewTCS = null;
    private readonly LinkedList<GameUpdate> PendingUpdates = [];

    private HubConnection? HubConnection;
    private bool IsConnected => HubConnection is not null && HubConnection.State == HubConnectionState.Connected;
    private bool IsConnecting = false;
    private bool IsNavigating = false;
    private readonly SemaphoreSlim ConnectLock = new(1, 1);

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly ConnectBoxViewModel ConnectBoxVM;
    private GameViewModel? GameVM;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GamePage() {
        ConnectBoxVM = new(new(
            Key: () => Code ?? "",
            DefaultCode: () => Code ?? "",
            OnPlay: new(PlayRequest),
            OnWatch: new(WatchRequest)
        ));
        GameVM = null;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnPageInitializedAsync() {
        await ConnectLock.WaitAsync();
        await Navigator.AddAfterEventListener(OnPageLeave);
        ConnectLock.Release();
    }

    protected override void OnPageParametersSet() {
        if (!IsConnecting) Code = URLCode;
    }

    protected override async ValueTask OnPageDisposeAsync() {
        await ConnectLock.WaitAsync();
        await DisposeGame();
        await Navigator.RemoveAfterEventListener(OnPageLeave);
        ConnectLock.Release();
    }

    // Connect methods --------------------------------------------------------------------------------------------------------------------
    private async Task PlayRequest(ConnectData data) {
        try {
            await ConnectLock.WaitAsync();
            await PageLoader.Show(PAGE_LOADER_TASK.GAME);
            PendingUpdates.Clear();
            if (!Auth.IsRegistered()) Auth.LogInAnonymous(data.Name, User.GenerateSkin());
            if (!await CreateConnection(data.Code)) throw new GameException();
        } catch {
            Notification.Error(I18N.T("Something went wrong."));
            await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
        } finally {
            ConnectLock.Release();
        }
    }

    private void WatchRequest(ConnectData data) {
        Notification.Info(I18N.T("This feature is not available yet."));
    }

    private void GameViewRendered() { GameViewTCS?.TrySetResult(); }

    private async Task<bool> CreateConnection(string code) {
        if (IsConnected) return true;
        try {
            // 1) Create data URL:
            var q = new QueryParams();
            q.Set(Game.CODE_ID, code);
            q.Set(User.USER_ID, JsonSerializer.Serialize(Auth.User));
            var hubURL = URL.SetQueryParams(URL.ToAbsolute(GAME_HUB.ROUTE_CULTURE()), q);
            // 2) Create HUB:
            HubConnection = new HubConnectionBuilder().WithUrl(hubURL).Build();
            // 3) Add events:
            HubConnection.On<Game, Player>(GAME_HUB.CONNECTION_SUCCESSFUL, ConnectionSuccessful);
            HubConnection.On<GamePlayUpdate>(GAME_HUB.GAME_PLAY_UPDATE, GameUpdate);
            HubConnection.On<PlayerUpdate>(GAME_HUB.PLAYER_UPDATE, GameUpdate);
            HubConnection.On<GameExceptionDTO>(GAME_HUB.ERROR, HandleErrors);
            HubConnection.Closed += OnConnectionClosed;
            // 4) Connect:
            await HubConnection.StartAsync();
        } catch (Exception) {
            await DisposeHub();
        }
        return IsConnected;
    }

    private async Task ConnectionSuccessful(Game game, Player player) {
        try {
            await ConnectLock.WaitAsync();
            await PageLoader.Show(PAGE_LOADER_TASK.GAME);
            IsConnecting = true;
            IsNavigating = true;
    
            // 1) Create ViewModel:
            GameVM = new(game, player, Send, new(GameViewRendered));

            // 2) Set URL:
            var state = Navigator.State<GamePageHistoryState>();
            bool isCodeSet = URLCode != null;
            if (isCodeSet) await Navigator.NavigateTo(I18N.Link<GamePage>([""]), replace: true);
            await Navigator.NavigateTo(
                I18N.Link<GamePage>([GameVM.Game.Code]),
                replace: state.WasRedirect && isCodeSet
            );
            Navigator.SetState<GamePageHistoryState>(new() { WasRedirect = true });

            // 3) Update and render:
            GameViewTCS = new TaskCompletionSource();
            IsNavigating = false;
            foreach (var update in PendingUpdates) {
                ExecuteUpdate(update);
            }
            StateHasChanged();
            await GameViewTCS.Task;

            // 4) Full screen:
            LayoutVM.HideNavigation();
            ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
        } catch {
            Notification.Error(I18N.T("Something went wrong."));
        } finally {
            IsConnecting = false;
            await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
            ConnectLock.Release();
        }
    }

    // Client actions ---------------------------------------------------------------------------------------------------------------------
    public async Task Send(string action, object message) {
        try {
            await ConnectLock.WaitAsync();
            if (HubConnection is null) return;
            await HubConnection.SendAsync(action, message);
        } finally {
            ConnectLock.Release();
        }
    }

    // Server actions ---------------------------------------------------------------------------------------------------------------------
    public void ExecuteUpdate(GameUpdate update) {
        GameVM?.Game?.Update(update);
        StateHasChanged();
    }

    public async Task GameUpdate(GameUpdate update) {
        try {
            await ConnectLock.WaitAsync();
            if (GameVM is null) {
                PendingUpdates.AddLast(update);
                return;
            }
            ExecuteUpdate(update);
        } finally {
            ConnectLock.Release();
        }
    }

    // Error handling ---------------------------------------------------------------------------------------------------------------------
    private async Task HandleErrors(GameExceptionDTO? exception = null) {
        await ConnectLock.WaitAsync();
        if (exception is null) await DisposeGame();
        else {
            if (exception.Type == GAME_EXCEPTION_TYPE.EXCEPTION) await DisposeGame();
            if (exception.Errors.Count > 0) {
                foreach (var error in exception.Errors) {
                    Notification.Error(error.Message);
                    Input<object>.TrySetError(error);
                }
            } else {
                Notification.Error(exception.Message);
            }
        }
        await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
        ConnectLock.Release();
    }

    private async Task OnConnectionClosed(Exception? e) {
        if (e is not null) await HandleErrors(
            new GameException([
                new Error(I18N.T("You have been disconnected from the server."))
            ])
            .DataTransferObject()
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
    }

    private async Task DisposeGame() {
        await DisposeHub();
        GameViewTCS = null;
        PendingUpdates.Clear();
        GameVM = null;
        if (!Auth.IsRegistered()) Auth.LogOut();
        LayoutVM.ShowNavigation();
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }
}
