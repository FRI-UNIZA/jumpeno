namespace Jumpeno.Client.ViewModels;

using System.Timers;

public class GameViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int PING_INTERVAL_MS = 2000;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Game Game { get; private set; }
    public Player Player { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public GameViewModel(Game game, Player player, LinkedList<GameUpdate> updates, Func<string, object, Task> send, EmptyDelegate onRender) {
        Game = game;
        Player = Game.GetPlayerRef(player.ID) ?? player;
        GameUpdates = updates;
        UpdateListLock = new();
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

    // Update Data ------------------------------------------------------------------------------------------------------------------------
    private LinkedList<GameUpdate> GameUpdates;
    private readonly LockerSlim UpdateListLock;
    private readonly LockerSlim UpdateLock;
    public bool Updating { get; private set; }

    public async Task AddUpdate(GameUpdate update) {
        await UpdateListLock.Lock();
        GameUpdates.AddLast(update);
        UpdateListLock.Unlock();
        if (Updating) await ExecuteUpdates();
    }

    public async Task ResetUpdates() {
        await UpdateListLock.Lock();
        GameUpdates.Clear();
        UpdateListLock.Unlock();
    }

    // Update execution -------------------------------------------------------------------------------------------------------------------
    public async Task ExecuteUpdates() {
        // 1) Take pending updates:
        await UpdateListLock.Lock();
        var updates = GameUpdates;
        GameUpdates = [];
        UpdateListLock.Unlock();
        // 2) Execute updates:
        await UpdateLock.Exclusive(async () => {
            if (BeforeUpdates != null) await BeforeUpdates.Invoke();
            foreach (var update in updates) {
                if (BeforeUpdate != null) await BeforeUpdate.Invoke(new(update));
                var success = Game.Update(update);
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
}
