namespace Jumpeno.Client.Services;

using System.Diagnostics;

#pragma warning disable CS8618
#pragma warning disable CA1816

public class Navigator : StaticService<Navigator>, IAsyncDisposable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly NavigationManager Manager;
    private static Action<string, bool, bool> ServerRedirect;
    private static Action ServerRefresh;
    // Stats:
    private string PreviousURL = "";
    private bool ProgramNavigation = false;
    private bool SettingQueries = false;
    // Data & state:
    private object? NavData = null;
    private (string Key, object? Data)? NavState = null;
    // Notify:
    private NotifyType? Notify = null;
    // Loading:
    private bool Loader = true;
    private const int MIN_LOADING = 175; // ms
    private readonly MinWatch MinLoadingWatch = new(MIN_LOADING);
    private TaskCompletionSource NavigationFinished;
    // PopState:
    private bool IsPopState = false;
    private readonly Stopwatch PopWatch = new();
    private readonly int POP_THROTTLE = 500; // ms
    // Events:
    private TaskCompletionSource NavEventTCS = new();
    private bool IsRunning = false;
    private readonly int RUN_DELAY = 100; // ms
    // Locks:
    private readonly LockerSlim NavLock = new();
    // Listeners & interceptors:
    private readonly List<Func<NavigationEvent, bool>> Blockers = [];
    private readonly List<EventDelegate<NavigationEvent>> AfterListeners = [];
    private readonly List<EventDelegate<NavigationEvent>> AfterFinishListeners = [];
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Navigator() {
        if (!AppEnvironment.IsServer) {
            Manager = AppEnvironment.GetService<NavigationManager>();
            Manager.RegisterLocationChangingHandler(BeforeLocationChanged);
            Manager.LocationChanged += AfterLocationChanged;
            PopWatch.Start();
        }
        Disposer = new(this, NavLock.DisposeSafe);
    }
    private readonly Disposer Disposer;
    public async ValueTask DisposeAsync() => await Disposer.DisposeAsync();
    ~Navigator() => Disposer.Final();

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(Action<string, bool, bool> serverRedirect, Action serverRefresh) {
        InitOnce.Check(nameof(Navigator));
        ServerRedirect = serverRedirect;
        ServerRefresh = serverRefresh;
    }

    public static void Init() => Init((url, forceLoad, replace) => {}, () => {});

    // Reset ------------------------------------------------------------------------------------------------------------------------------
    private void ResetStats() {
        PreviousURL = "";
        ProgramNavigation = false;
        SettingQueries = false;
        NavData = null;
        NavState = null;
        Notify = null;
        IsPopState = false;
        IsRunning = false;
    }

    private void Release() {
        ResetStats(); Loader = true;
        NavEventTCS.TrySetResult();
        NavigationFinished?.TrySetResult();
        NavLock.TryUnlock();
    }

    private async Task Terminate() {
        Release();
        await PageLoader.Hide(PageLoaderTask.Navigator, false);
    }

    private void PreventNavigation(LocationChangingContext ctx) {
        ctx.PreventNavigation();
        Release();
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private async ValueTask BeforeLocationChanged(LocationChangingContext ctx) {
        try {
            // 1) Check allowed:
            CheckAllowed();
            // 2) Diff [sync (navigation can not be prevented after async call)]:
            if (!ProgramNavigation) {
                if (IsRunning) { ctx.PreventNavigation(); return; }
                if (!ctx.IsNavigationIntercepted) {
                    if (PopWatch.ElapsedMilliseconds < POP_THROTTLE) {
                        ctx.PreventNavigation(); return;
                    }
                    IsPopState = true;
                }
            }
            PopWatch.Restart();
            // 3) Set running:
            IsRunning = true;
            // 4) Run blockers:
            foreach (var blocker in Blockers) {
                if (!blocker(new(ProgramNavigation, IsPopState, PreviousURL, ctx.TargetLocation))) {
                    PreventNavigation(ctx); return;
                }
            }
            // 5) Create event (ensure run before AfterChange):
            NavEventTCS = new();
            // 6) Lock if not program:
            if (!ProgramNavigation) await NavLock.TryLock();
            // 7) Remember URL:
            PreviousURL = URL.Url();
            // 8) Loader:
            if (Loader) {
                await PageLoader.Show(PageLoaderTask.Navigator);
                MinLoadingWatch.Start();
            } else {
                await PageLoader.Show(PageLoaderTask.Navigator, true);   
            }
            // 9) Check cancellation:
            ctx.CancellationToken.ThrowIfCancellationRequested();
            // 10) Set event:
            NavEventTCS.TrySetResult();
        } catch {
            // 11) Terminate on error:
            await Terminate();
        }
    }

    private async void AfterLocationChanged (object? sender, LocationChangedEventArgs e) {
        try {
            // 1) Await event from before:
            await NavEventTCS.Task;
            // 2) Run listeners:
            foreach (var listener in AfterListeners) {
                await listener.Invoke(new(ProgramNavigation, IsPopState, PreviousURL, e.Location));
            }
            // 3) Notify:
            if (Notify is NotifyType notify) {
                AppLayout.Notify(notify);
                NavigationFinished.TrySetResult();
            } else {
                var samePage = URL.PathMatches(URL.Path(PreviousURL), URL.Path(e.Location));
                if (ProgramNavigation) {
                    if (!SettingQueries && URL.IsLocal(e.Location)) {
                        AppLayout.Notify(samePage ? NotifyType.Page : NotifyType.State);
                    }
                    NavigationFinished.TrySetResult();
                } else {
                    AppLayout.Notify(samePage ? NotifyType.Page : NotifyType.State);
                }
            }
            // 4) Set state:
            if (NavState != null) SetState(NavState.Value.Key, NavState.Value.Data);
            // 5) Reset stats:
            ResetStats();
            // 6) Handle loader:
            if (Loader) await MinLoadingWatch.Task;
            await PageLoader.Hide(PageLoaderTask.Navigator, false);
            Loader = true;
            // 7) Run after listeners:
            foreach (var listener in AfterFinishListeners) {
                await listener.Invoke(new(ProgramNavigation, IsPopState, PreviousURL, e.Location));
            }
            // 8) Unlock:
            NavLock.TryUnlock();
        } catch {
            // 9) Terminate on error:
            await Terminate();
        }
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private async Task Navigate(
        string url,
        bool forceLoad = false, bool replace = false, bool queries = false,
        object? data = null, (string Key, object? Data)? state = null, NotifyType? notify = null,
        bool loader = true
    ) {
        // 1) Set running:
        if (AppEnvironment.IsClient) {
            while (IsRunning) await Task.Delay(RUN_DELAY);
            IsRunning = true;
        }
        // 2) Lock program navigation:
        await NavLock.TryLock();
        // 3) Show program loader before start:
        if (AppEnvironment.IsClient) {
            if (loader) await PageLoader.Show(PageLoaderTask.Navigator);
        }
        // 4) Handle server:
        if (AppEnvironment.IsServer) {
            ServerRedirect(url, forceLoad, replace);
            RequestStorage.Set(RequestStorages.URL, url);
            NavLock.TryUnlock();
            return;
        }
        // 5) Set stats:
        ProgramNavigation = true;
        SettingQueries = queries;
        Loader = loader;
        NavData = data;
        NavState = AppEnvironment.IsClient ? state : null;
        Notify = notify;
        NavigationFinished = new TaskCompletionSource();
        // 6) Handle culture:
        if (!queries && URL.IsLocal(url)) {
            if (!I18N.IsCurrentCultureUrl(url)) forceLoad = true;
        }
        // 7) Navigate:
        Manager.NavigateTo(url, forceLoad, replace);
        // 8) Wait until finished:
        await NavigationFinished.Task;
    }

    public static async Task NavigateTo(
        string url, bool forceLoad = false, bool replace = false,
        object? data = null, (string Key, object? Data)? state = null, NotifyType? notify = null
    ) => await Instance().Navigate(
        url,
        forceLoad, replace, queries: false,
        data, state, notify,
        loader: !forceLoad
    );
    public static void Refresh() {
        if (AppEnvironment.IsServer) ServerRefresh();
        else Instance().Manager.Refresh(forceReload: true);
    }
    public static async Task SetQueryParams(QueryParams queryParams) => await Instance().Navigate(
        URL.SetQueryParams(queryParams),
        forceLoad: false, replace: true, queries: true,
        data: null, state: null, notify: null,
        loader: false
    );

    // NOTE: Accessible only in constructor!
    public static T? Data<T>() {
        try { return (T?)Instance().NavData; }
        catch { return default; }
    }
    public static T Data<T>(T fallback) {
        try { return (T?)Instance().NavData ?? fallback; }
        catch { return fallback; }
    }

    // NOTE: Can also be set in NavitageTo and is client only!
    public static T? State<T>(string key) => AppEnvironment.IsClient ? JS.Invoke<T?>(JSNavigator.State, key) : default;
    public static T State<T>(string key, T fallback) => AppEnvironment.IsClient ? JS.Invoke<T?>(JSNavigator.State, key) ?? fallback : fallback;
    public static void SetState<T>(string key, T state, string? url = null) { if (AppEnvironment.IsClient) JS.InvokeVoid(JSNavigator.SetState, key, state, url); }

    // Assert [Browser navigation interference] -------------------------------------------------------------------------------------------
    private static uint Counter = 0;
    private static byte? AllowedCount = null;
    private static void CheckAllowed() { if (AllowedCount != null && ++Counter > AllowedCount) Refresh(); }
    // Actions:
    public static void AllowNone() { AllowedCount = 0; Counter = 0; }
    public static void AllowOne() { AllowedCount = 1; Counter = 0; }
    public static void AllowAny() { AllowedCount = null; Counter = 0; }

    // Listeners --------------------------------------------------------------------------------------------------------------------------
    public static async Task AddBlocker(Func<NavigationEvent, bool> predicate) {
        var instance = Instance();
        await instance.NavLock.TryExclusive(() => instance.Blockers.Add(predicate));
    }

    public static async Task RemoveBlocker(Func<NavigationEvent, bool> predicate) {
        var instance = Instance();
        await instance.NavLock.TryExclusive(() => {
            for (int i = 0; i < instance.Blockers.Count; i++) {
                if (!predicate.Equals(instance.Blockers[i])) continue;
                instance.Blockers.RemoveAt(i); break;
            }
        });
    }

    public static async Task AddAfterEventListener(EventDelegate<NavigationEvent> listener) {
        var instance = Instance(); await instance.NavLock.TryExclusive(() => instance.AfterListeners.Add(listener));
    }
    public static async Task AddAfterEventListener(Action<NavigationEvent> listener) {
        await AddAfterEventListener(new EventDelegate<NavigationEvent>(listener));
    }
    public static async Task AddAfterEventListener(Func<NavigationEvent, Task> listener) {
        await AddAfterEventListener(new EventDelegate<NavigationEvent>(listener));
    }

    public static async Task RemoveAfterEventListener(EventDelegate<NavigationEvent> listener) {
        var instance = Instance(); await instance.NavLock.TryExclusive(() => {
            for (int i = 0; i < instance.AfterListeners.Count; i++) {
                if (!listener.Equals(instance.AfterListeners[i])) continue;
                instance.AfterListeners.RemoveAt(i); break;
            }
        });
    }
    public static async Task RemoveAfterEventListener(Action<NavigationEvent> listener) {
        await RemoveAfterEventListener(new EventDelegate<NavigationEvent>(listener));
    }
    public static async Task RemoveAfterEventListener(Func<NavigationEvent, Task> listener) {
        await RemoveAfterEventListener(new EventDelegate<NavigationEvent>(listener));
    }

    public static async Task AddAfterFinishEventListener(EventDelegate<NavigationEvent> listener) {
        var instance = Instance(); await instance.NavLock.TryExclusive(() => instance.AfterFinishListeners.Add(listener));
    }
    public static async Task AddAfterFinishEventListener(Action<NavigationEvent> listener) {
        await AddAfterFinishEventListener(new EventDelegate<NavigationEvent>(listener));
    }
    public static async Task AddAfterFinishEventListener(Func<NavigationEvent, Task> listener) {
        await AddAfterFinishEventListener(new EventDelegate<NavigationEvent>(listener));
    }

    public static async Task RemoveAfterFinishEventListener(EventDelegate<NavigationEvent> listener) {
        var instance = Instance(); await instance.NavLock.TryExclusive(() => {
            for (int i = 0; i < instance.AfterFinishListeners.Count; i++) {
                if (!listener.Equals(instance.AfterFinishListeners[i])) continue;
                instance.AfterFinishListeners.RemoveAt(i); break;
            }
        });
    }
    public static async Task RemoveAfterFinishEventListener(Action<NavigationEvent> listener) {
        await RemoveAfterFinishEventListener(new EventDelegate<NavigationEvent>(listener));
    }
    public static async Task RemoveAfterFinishEventListener(Func<NavigationEvent, Task> listener) {
        await RemoveAfterFinishEventListener(new EventDelegate<NavigationEvent>(listener));
    }
}
