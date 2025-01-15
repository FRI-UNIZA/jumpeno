namespace Jumpeno.Client.ViewModels;

using System.Collections.Concurrent;
using System.Timers;

public class GameViewModel : IDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int PING_INTERVAL_MS = 2000;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string QRCode { get; private set; }
    public Game Game { get; private set; }
    public Player? Player { get; private set; }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool IsHost => Game.Host.Equals(Auth.User);
    public bool IsWatching => Game.DisplayMode == DISPLAY_MODE.EACH_OWN || !IsPlayer || IsHost;
    public bool IsPlayer => Player != null;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameViewModel(
        string qrCode, Game game, Player? player,
        LinkedList<GameUpdate> updates, Func<string, object, Task> send, EmptyDelegate onRender
    ) {
        QRCode = qrCode;
        Game = game;
        Player = player == null ? player : Game.GetPlayerRef(player.ID);
        GameUpdates = InitGameUpdates(updates);
        Send = send;
        OnRender = onRender;
    }

    public void Dispose() {
        UpdateLock.Dispose();
        PingTimer?.Dispose();
        PingLock.Dispose();
        GC.SuppressFinalize(this);
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    private readonly EmptyDelegate OnRender;

    // NOTE: Init must be called after render of displayed component:
    public async Task InitOnRender() => await OnRender.Invoke();

    public async Task PreRender() {
        if (!IsWatching) return;
        await Game.Map.PreRender(Game);
        await Game.Map.Shrink.PreRender(Game);
    }

    private static ConcurrentQueue<GameUpdate> InitGameUpdates(LinkedList<GameUpdate> updates) {
        var queue = new ConcurrentQueue<GameUpdate>();
        foreach (var update in updates) queue.Enqueue(update);
        return queue;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static async Task Request(Func<Task> request) => await PageLoader.Try(request, PAGE_LOADER_TASK.GAME_REQUEST);

    // Update Data ------------------------------------------------------------------------------------------------------------------------
    private readonly ConcurrentQueue<GameUpdate> GameUpdates;
    private readonly LinkedList<GameUpdate> FutureUpdates = [];
    private readonly LockerSlim UpdateLock = new();
    public bool Updating { get; private set; } = false;

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
            if ((gameUpdate.Round == Game.Round && Game.LOBBY_STATES.Contains(Game.State)) || (gameUpdate.Round > Game.Round)) {
                FutureUpdates.AddLast(gameUpdate);
                return false;
            }
        }
        // 2) Update game:
        return Game.Update(update);
    }

    public async Task ExecuteUpdates() {
        await UpdateLock.TryExclusive(async () => {
            if (BeforeUpdates != null) await BeforeUpdates.Invoke();
            FutureUpdates.Clear();
            while (GameUpdates.TryDequeue(out var update)) {
                if (BeforeUpdate != null) await BeforeUpdate.Invoke(new(update));
                var success = TryUpdateGame(update);
                if (AfterUpdate != null) await AfterUpdate.Invoke(new(update, success));
            }
            foreach (var update in FutureUpdates) GameUpdates.Enqueue(update);
            if (AfterUpdates != null) await AfterUpdates.Invoke();
            if (Notify != null) await Notify.Invoke();
        });
    }

    public async Task StartUpdating() {
        Updating = true;
        await ExecuteUpdates();
    }

    public void StopUpdating() {
        Updating = false;
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

    private event Func<Task>? Notify;
    public async Task AddNotifyListener(Func<Task> listener) => await UpdateLock.TryExclusive(() => Notify += listener);
    public async Task RemoveNotifyListener(Func<Task> listener) => await UpdateLock.TryExclusive(() => Notify -= listener);

    // Server communication ---------------------------------------------------------------------------------------------------------------
    public Func<string, object, Task> Send { get; private set; }
    public async Task SendGameUpdate(NetworkUpdate update) => await Send(update.HUB_ACTION, update);

    // Ping -------------------------------------------------------------------------------------------------------------------------------
    private Timer? PingTimer = null;
    private readonly LockerSlim PingLock = new();

    // Ping - actions:
    public async Task SendPing() {
        var update = Game.NewPingUpdate();
        await Send(update.HUB_ACTION, update);
    }

    public async Task StartPing() {
        await PingLock.TryExclusive(() => {
            PingTimer = new(PING_INTERVAL_MS);
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

    // Controls ---------------------------------------------------------------------------------------------------------------------------
    public bool ControlsDisplayed { get; private set; } = false;
    public void InitControls() => ControlsDisplayed = Player != null && Player.Device == DEVICE_TYPE.TOUCH;
    public void ToggleControls() => ControlsDisplayed = !ControlsDisplayed;
}
