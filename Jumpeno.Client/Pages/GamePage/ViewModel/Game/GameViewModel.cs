namespace Jumpeno.Client.ViewModels;

using System.Collections.Concurrent;
using System.Timers;

public class GameViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int PING_INTERVAL_MS = 2000;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Game Game { get; private set; }
    public Player? Player { get; private set; }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool IsHost => Game.Host.Equals(Auth.User);
    public bool IsWatching => Game.DisplayMode == DISPLAY_MODE.EACH_OWN || !IsPlayer || IsHost;
    public bool IsPlayer => Player != null;

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameViewModel(Game game, Player? player, LinkedList<GameUpdate> updates, Func<string, object, Task> send, EmptyDelegate onRender) {
        Game = game;
        Player = player == null ? player : Game.GetPlayerRef(player.ID);
        GameUpdates = InitGameUpdates(updates);
        UpdateLock = new();
        PingTimer = null;
        Send = send;
        Updating = false;
        OnRender = onRender;
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    private readonly EmptyDelegate OnRender;

    // NOTE: Init must be called after render of displayed component:
    public async Task InitOnRender() => await OnRender.Invoke();

    private static ConcurrentQueue<GameUpdate> InitGameUpdates(LinkedList<GameUpdate> updates) {
        var queue = new ConcurrentQueue<GameUpdate>();
        foreach (var update in updates) queue.Enqueue(update);
        return queue;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static async Task GameRequest(Func<Task> action) {
        await PageLoader.Show(PAGE_LOADER_TASK.GAME_REQUEST);
        try { await action(); }
        catch { Console.Error.WriteLine("Game request failed!"); }
        finally { await PageLoader.Hide(PAGE_LOADER_TASK.GAME_REQUEST); }
    }

    // Update Data ------------------------------------------------------------------------------------------------------------------------
    private readonly ConcurrentQueue<GameUpdate> GameUpdates;
    private readonly LockerSlim UpdateLock;
    public bool Updating { get; private set; }

    public async Task AddUpdate(GameUpdate update) {
        GameUpdates.Enqueue(update);
        if (Updating) await ExecuteUpdates();
    }

    public void ResetUpdates() => GameUpdates.Clear();

    // Update execution -------------------------------------------------------------------------------------------------------------------
    private bool TryUpdateGame(GameUpdate update) {
        // 1) Ensure current round:
        if (update is GamePlayUpdate gameUpdate) {
            if (gameUpdate.Round < Game.Round) return false;
            if ((gameUpdate.Round == Game.Round && Game.LOBBY_STATES.Contains(Game.State)) || (gameUpdate.Round > Game.Round)) {
                GameUpdates.Enqueue(gameUpdate);
                return false;
            }
        }
        // 2) Update game:
        return Game.Update(update);
    }

    public async Task ExecuteUpdates() {
        await UpdateLock.Exclusive(async () => {
            if (BeforeUpdates != null) await BeforeUpdates.Invoke();
            while (GameUpdates.TryDequeue(out var update)) {
                if (BeforeUpdate != null) await BeforeUpdate.Invoke(new(update));
                var success = TryUpdateGame(update);
                if (AfterUpdate != null) await AfterUpdate.Invoke(new(update, success));
            }
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
    public async Task AddBeforeUpdatesListener(Func<Task> listener) { await UpdateLock.Lock(); BeforeUpdates += listener; UpdateLock.Unlock(); }
    public async Task RemoveBeforeUpdatesListener(Func<Task> listener) { await UpdateLock.Lock(); BeforeUpdates -= listener; UpdateLock.Unlock(); }
    
    private event Func<UpdateBeforeEvent, Task>? BeforeUpdate;
    public async Task AddBeforeUpdateListener(Func<UpdateBeforeEvent, Task>? listener) { await UpdateLock.Lock(); BeforeUpdate += listener; UpdateLock.Unlock(); }
    public async Task RemoveBeforeUpdateListener(Func<UpdateBeforeEvent, Task>? listener) { await UpdateLock.Lock(); BeforeUpdate -= listener; UpdateLock.Unlock(); }
    
    private event Func<UpdateAfterEvent, Task>? AfterUpdate;
    public async Task AddAfterUpdateListener(Func<UpdateAfterEvent, Task>? listener) { await UpdateLock.Lock(); AfterUpdate += listener; UpdateLock.Unlock(); }
    public async Task RemoveAfterUpdateListener(Func<UpdateAfterEvent, Task>? listener) { await UpdateLock.Lock(); AfterUpdate -= listener; UpdateLock.Unlock(); }
    
    private event Func<Task>? AfterUpdates;
    public async Task AddAfterUpdatesListener(Func<Task> listener) { await UpdateLock.Lock(); AfterUpdates += listener; UpdateLock.Unlock(); }
    public async Task RemoveAfterUpdatesListener(Func<Task> listener) { await UpdateLock.Lock(); AfterUpdates -= listener; UpdateLock.Unlock(); }

    private event Func<Task>? Notify;
    public async Task AddNotifyListener(Func<Task> listener) { await UpdateLock.Lock(); Notify += listener; UpdateLock.Unlock(); }
    public async Task RemoveNotifyListener(Func<Task> listener) { await UpdateLock.Lock(); Notify -= listener; UpdateLock.Unlock(); }

    // Server communication ---------------------------------------------------------------------------------------------------------------
    public Func<string, object, Task> Send { get; private set; }
    public async Task SendGameUpdate(NetworkUpdate update) => await Send(update.HUB_ACTION, update);

    // Ping -------------------------------------------------------------------------------------------------------------------------------
    private Timer? PingTimer;
    private readonly LockerSlim PingLock = new();

    // Ping - actions:
    public async Task SendPing() {
        var update = Game.NewPingUpdate();
        await Send(update.HUB_ACTION, update);
    }

    public async Task StartPing() {
        await PingLock.Exclusive(() => {
            PingTimer = new(PING_INTERVAL_MS);
            PingTimer.Elapsed += async (sender, e) => await SendPing();
            PingTimer.Start();
        });
    }

    public async Task StopPing() {
        await PingLock.Exclusive(() => {
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
