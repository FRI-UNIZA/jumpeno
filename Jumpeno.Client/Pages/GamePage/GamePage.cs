namespace Jumpeno.Client.Pages;

using Microsoft.AspNetCore.SignalR.Client;

public partial class GamePage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ROUTE_EN = "/en/game/{URLCode?}";
    public const string ROUTE_SK = "/sk/hra/{URLCode?}";
    public static string Link(string url, string code) {
        return URL.ReplaceSegments(url, new() {{ 1, $"{code}" }});
    }

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public MainLayoutController? MainLayoutController { get; set; } = null;
    [Parameter]
    public string? URLCode { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string? Code = null;

    private GAME_PAGE_STATE State = GAME_PAGE_STATE.CONNECT;
    private TaskCompletionSource? LobbyTCS = null;

    private HubConnection? HubConnection;
    private bool IsConnected => HubConnection is not null && HubConnection.State == HubConnectionState.Connected;
    private bool IsConnecting = false;
    private readonly SemaphoreSlim ConnectLock = new(1, 1);

    private Game? Game;
    private TaskCompletionSource<List<string>>? GameTCS;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly ConnectBoxViewModel ConnectBoxVM;
    private readonly LobbyViewModel LobbyVM;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GamePage() {
        // ConnectBox ---------------------------------------------------------------------------------------------------------------------
        ConnectBoxVM = new(new(
            Key: () => Code ?? "",
            DefaultCode: () => Code ?? "",
            OnPlay: new(async Task (ConnectData data) => {
                try {
                    await ConnectLock.WaitAsync();
                    await PageLoader.Show(PAGE_LOADER_TASK.GAME_CONNECT);
                    if (!Auth.IsLoggedIn()) {
                        Auth.LoginAnonymous(data.Name, (SKIN) new Random().Next(Enum.GetValues(typeof(SKIN)).Length));
                    }
                    
                    if (!await CreateConnection()) {
                        Notification.Error(I18N.T("Something went wrong."));
                        return;
                    }

                    var errors = await ConnectToGame(data.Code, Auth.User!);
                    if (errors.Count > 0) {
                        foreach (var error in errors) Notification.Error(error);
                        return;
                    }

                    IsConnecting = true;

                    var state = Navigator.State<GamePageHistoryState>();
                    bool isCodeSet = URLCode != null;
                    if (isCodeSet) await Navigator.NavigateTo(I18N.Link<GamePage>([""]), replace: true);
                    await Navigator.NavigateTo(
                        I18N.Link<GamePage>([data.Code]),
                        replace: state.WasRedirect && isCodeSet
                    );
                    Navigator.SetState<GamePageHistoryState>(new() { WasRedirect = true });

                    LobbyTCS = new TaskCompletionSource();
                    State = GAME_PAGE_STATE.LOBBY;
                    StateHasChanged();
                    await LobbyTCS.Task;

                    MainLayoutController?.HideNavigation();
                    ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);

                    IsConnecting = false;
                } catch {
                    Notification.Error(I18N.T("Something went wrong."));
                } finally {
                    await PageLoader.Hide(PAGE_LOADER_TASK.GAME_CONNECT);
                    ConnectLock.Release();
                }
            }),
            OnWatch: new((ConnectData data) => {
                Notification.Info(I18N.T("This feature is not available yet."));
            })
        ));
        // Lobby --------------------------------------------------------------------------------------------------------------------------
        LobbyVM = new(new(
            OnRender: new(() => {
                LobbyTCS?.TrySetResult();
            })
        ));
    }

    // Connect methods --------------------------------------------------------------------------------------------------------------------
    private async Task<bool> CreateConnection() {
        if (IsConnected) return true;
        try {
            HubConnection = new HubConnectionBuilder().WithUrl(URL.ToAbsolute(GAME_HUB.ROUTE_CULTURE())).Build();
            HubConnection.On<Game?, List<string>>(GAME_HUB.CONNECT_TO_GAME_RESPONSE, (game, errors) => {
                Game = game;
                GameTCS?.TrySetResult(errors);
            });
            HubConnection.Closed += OnConnectionClosed;
            await HubConnection.StartAsync();
        } catch (Exception) {
            HubConnection = null;
        }
        return IsConnected;
    }

    private async Task<List<string>> ConnectToGame(string code, User user) {
        if (!IsConnected) throw new InvalidOperationException("HubConnection not ready!");
        GameTCS = new();
        await HubConnection!.SendAsync(GAME_HUB.CONNECT_TO_GAME_REQUEST, code, user);
        return await GameTCS.Task;
    }

    // Dispose methods --------------------------------------------------------------------------------------------------------------------
    private async Task DisposeGame() {
        if (HubConnection is not null) {
            await HubConnection.DisposeAsync();
            HubConnection = null;
        }
        State = GAME_PAGE_STATE.CONNECT;
        Game = null;
        MainLayoutController?.ShowNavigation();
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }

    private async Task OnConnectionClosed(Exception? e) {
        await ConnectLock.WaitAsync();
        if (e is not null) {
            await Navigator.NavigateTo(URL.Url(), replace: true);
            await DisposeGame();
            Notification.Error(I18N.T("You have been disconnected from the server."));
            await PageLoader.Hide(PAGE_LOADER_TASK.GAME_CONNECT);
        }
        ConnectLock.Release();
    }

    private async Task OnPageLeave(NavigationEvent e) {
        var pagePath = I18N.Link<GamePage>([""]);
        if (!((URL.Path(e.BeforeURL) != pagePath) && (URL.Path(e.AfterURL) == pagePath)) || IsConnecting) {
            return;
        }
        await ConnectLock.WaitAsync();
        await DisposeGame();
        ConnectLock.Release();
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
}
