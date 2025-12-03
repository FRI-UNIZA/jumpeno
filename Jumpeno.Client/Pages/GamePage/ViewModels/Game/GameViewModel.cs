namespace Jumpeno.Client.ViewModels;

using System.Timers;

#pragma warning disable CA1822

public class GameViewModel : IAsyncDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static int PING_INTERVAL => From.SToMS(AppSettings.Game.PingInterval.Seconds); // ms
    // Classes:
    public const string CLASS_HOST = "host";
    public const string CLASS_WATCHING = "watching";
    public const string CLASS_PLAYER = "player";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string QRCode { get; private set; }
    public Game Game { get; private set; }
    public Player? Player { get; private set; }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool IsHost => Game.Host.Equals(Auth.User);
    public bool IsWatching => Game.DisplayMode == DISPLAY_MODE.EACH_OWN || !IsPlayer || IsHost;
    public bool IsPlayer => Player != null;
    
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public CSSClass CSSClass() {
        return new CSSClass()
        .Set(CLASS_HOST, IsHost)
        .Set(CLASS_WATCHING, IsWatching)
        .Set(CLASS_PLAYER, IsPlayer);
    }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public ChatViewModel Chat { get; private set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameViewModel(
        string qrCode, Game game, Player? player,
        LinkedList<GameUpdate> updates,
        Func<string, object, Task> send, Func<string, object, Task> sendRequest, Func<EmptyDelegate, Task> exec,
        Func<GameChat?> chat, Action notify
    ) {
        QRCode = qrCode;
        Game = game;
        Player = player;
        GameUpdates = InitGameUpdates(updates);
        Send = send;
        SendRequest = sendRequest;
        Exec = action => exec(new(action));
        ExecAsync = action => exec(new(action));
        Chat = new(Game.Code, chat, notify);
        Notify = notify;
        Ping = null;
    }

    public async ValueTask DisposeAsync() {
        await UpdateLock.DisposeSafe();
        PingTimer?.Dispose();
        await PingLock.DisposeSafe();
        await DisposeChat();
        GC.SuppressFinalize(this);
    }

    // Initialization [Chat] --------------------------------------------------------------------------------------------------------------
    public async Task InitChat() => await Chat.Init();
    private async Task DisposeChat() => await Chat.Dispose();

    // Initialization [Render] ------------------------------------------------------------------------------------------------------------
    public async Task PreRender() {
        if (!IsWatching) return;
        await Game.Map.PreRender();
        await Game.Map.Shrink.PreRender(Game);
    }

    // Initialization [UI] ----------------------------------------------------------------------------------------------------------------
    public void InitUI() {
        BlockUserActions();
        InitControls();
    }

    public void DisposeUI() => AllowUserActions();

    // Update Data ------------------------------------------------------------------------------------------------------------------------
    private readonly ConcurrentQueue<GameUpdate> GameUpdates;
    private readonly LinkedList<GameUpdate> FutureUpdates = [];
    private readonly LockerSlim UpdateLock = new();
    public bool Updating { get; private set; } = false;

    private static ConcurrentQueue<GameUpdate> InitGameUpdates(LinkedList<GameUpdate> updates) {
        var queue = new ConcurrentQueue<GameUpdate>();
        foreach (var update in updates) queue.Enqueue(update);
        return queue;
    }

    public async Task AddUpdate(GameUpdate update) {
        GameUpdates.Enqueue(update);
        if (Updating) await ExecuteUpdates();
    }

    public async Task ResetUpdates() => await UpdateLock.TryExclusive(GameUpdates.Clear);

    // Update execution -------------------------------------------------------------------------------------------------------------------
    private bool TryUpdateGame(GameUpdate update) {
        // 1) Ensure current round:
        if (update is GamePlayUpdate gameUpdate) {
            if (gameUpdate.Round < Game.Round) return false;
            if (gameUpdate.Round > Game.Round) {
                FutureUpdates.AddLast(gameUpdate);
                return false;
            }
        }
        // 2) Update game:
        return Game.Update(update);
    }

    public async Task ExecuteUpdates() {
        await UpdateLock.TryExclusive(async () => {
            if (BeforeUpdates != null) await BeforeUpdates();
            FutureUpdates.Clear();
            while (GameUpdates.TryDequeue(out var update)) {
                if (BeforeUpdate != null) await BeforeUpdate(new(update));
                var success = TryUpdateGame(update);
                if (AfterUpdate != null) await AfterUpdate(new(update, success));            
            }
            foreach (var update in FutureUpdates) GameUpdates.Enqueue(update);
            if (AfterUpdates != null) await AfterUpdates();
            Notify();
        });
    }

    public async Task StartUpdating() {
        Updating = true;
        Game.ClockAutoResetOn();
        await ExecuteUpdates();
    }

    public void StopUpdating() {
        Updating = false;
        Game.ClockAutoResetOff();
    }

    // Update events ----------------------------------------------------------------------------------------------------------------------
    private event Func<Task>? BeforeUpdates;
    public async Task AddBeforeUpdatesListener(Func<Task> listener) => await UpdateLock.TryExclusive(() => BeforeUpdates += listener);
    public async Task RemoveBeforeUpdatesListener(Func<Task> listener) => await UpdateLock.TryExclusive(() => BeforeUpdates -= listener);
    
    private event Func<UpdateBeforeEvent, Task>? BeforeUpdate;
    public async Task AddBeforeUpdateListener(Func<UpdateBeforeEvent, Task>? listener) => await UpdateLock.TryExclusive(() => BeforeUpdate += listener);
    public async Task RemoveBeforeUpdateListener(Func<UpdateBeforeEvent, Task>? listener) => await UpdateLock.TryExclusive(() => BeforeUpdate -= listener);
    
    private event Func<UpdateAfterEvent, Task>? AfterUpdate;
    public async Task AddAfterUpdateListener(Func<UpdateAfterEvent, Task>? listener) => await UpdateLock.TryExclusive(() => AfterUpdate += listener);
    public async Task RemoveAfterUpdateListener(Func<UpdateAfterEvent, Task>? listener) => await UpdateLock.TryExclusive(() => AfterUpdate -= listener);
    private event Func<Task>? AfterUpdates;
    public async Task AddAfterUpdatesListener(Func<Task> listener) => await UpdateLock.TryExclusive(() => AfterUpdates += listener);
    public async Task RemoveAfterUpdatesListener(Func<Task> listener) => await UpdateLock.TryExclusive(() => AfterUpdates -= listener);

    private readonly Action Notify;

    // Server communication ---------------------------------------------------------------------------------------------------------------
    public Func<string, object, Task> Send { get; private set; }
    public async Task SendGameUpdate(NetworkUpdate update) => await Send(update.HUB_ACTION, update);
    public async Task SendTripUpdate(GameTripUpdate update) => await Send(update.HUB_ACTION, update);
    public Func<string, object, Task> SendRequest { get; private set; }
    public async Task SendGameRequest(GameRequestUpdate update) => await SendRequest(update.HUB_ACTION, update);
    public Func<Action, Task> Exec { get; private set; }
    public Func<Func<Task>, Task> ExecAsync { get; private set; }

    // Ping -------------------------------------------------------------------------------------------------------------------------------
    public double? Ping { get; private set; }
    private Timer? PingTimer = null;
    private readonly LockerSlim PingLock = new();

    public void SetPing(PingUpdate update) {
        update.SetReturn();
        if (update.ReturnedAt is not DateTime returnedAt) return;
        Ping = GameClock.Delta(returnedAt, update.CreatedAt);
        Notify();
    }

    public async Task SendPing() => await SendTripUpdate(new PingUpdate(DateTime.UtcNow));

    public async Task StartPing() {
        await PingLock.TryExclusive(() => {
            PingTimer = new(PING_INTERVAL);
            PingTimer.Elapsed += async (sender, e) => await SendPing();
            PingTimer.Start();
        });
    }

    public async Task StopPing() {
        await PingLock.TryExclusive(() => {
            PingTimer?.Stop();
            PingTimer?.Dispose();
            PingTimer = null;
        });
    }

    // User actions -----------------------------------------------------------------------------------------------------------------------
    private static void BlockUserActions() {
        Window.BlockUserSelect();
        Window.TouchActionPanOn();
        Window.OverscrollNoneOn();
        Window.PreventTouchStart();
        Window.PreventTouchEnd();
    }
    public async Task SwitchToGameInput() => await Exec(BlockUserActions);

    private static void AllowUserActions() {
        Window.AllowUserSelect();
        Window.TouchActionPanOff();
        Window.OverscrollNoneOff();
        Window.DefaultTouchStart();
        Window.DefaultTouchEnd();
    }
    public async Task SwitchToWebInput() => await Exec(AllowUserActions);

    // Game -------------------------------------------------------------------------------------------------------------------------------
    public async Task Toggle() => await SendGameRequest(new GameActionRequestUpdate(GAME_ACTION.TOGGLE));
    public async Task Pause() => await SendGameRequest(new GameActionRequestUpdate(GAME_ACTION.PAUSE));
    public async Task Delete() => await SendGameRequest(new GameActionRequestUpdate(GAME_ACTION.DELETE));

    // Lobby ------------------------------------------------------------------------------------------------------------------------------
    private int? LobbyRound = null;
    public bool LobbyDisplayed => LobbyRound == Game.Round && Game.RUN_STATES.Contains(Game.State);

    public async Task ShowLobby() => await UpdateLock.TryExclusive(() => { LobbyRound = Game.Round; Notify(); });

    public async Task HideLobby() => await UpdateLock.TryExclusive(() => { LobbyRound = null; Notify(); });

    // Controls ---------------------------------------------------------------------------------------------------------------------------
    public bool ControlsDisplayed { get; private set; } = false;

    private void InitControls() {
        ControlsDisplayed = Player != null && (
            Player.Device == DEVICE_TYPE.TOUCH ||
            (Game.DisplayMode != DISPLAY_MODE.EACH_OWN && !IsHost)
        );
    }

    public void ToggleControls() { ControlsDisplayed = !ControlsDisplayed; Notify(); }
}
