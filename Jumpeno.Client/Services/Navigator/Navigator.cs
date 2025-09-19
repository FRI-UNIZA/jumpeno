namespace Jumpeno.Client.Services;

#pragma warning disable CS8618

public class Navigator : StaticService<Navigator>, IDisposable {
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
    private object? NavState = null;
    // Notify:
    private NOTIFY? Notify = null;
    // Loading:
    private bool Loader = true;
    private const int MIN_LOADING = 175; // ms
    private readonly MinWatch MinLoadingWatch = new(MIN_LOADING);
    private TaskCompletionSource NavigationFinished;
    // Locks:
    private readonly LockerSlim NavLock = new();
    // Listeners & interceptors:
    private readonly List<EventPredicate<NavigationEvent>> Blockers = [];
    private readonly List<EventDelegate<NavigationEvent>> BeforeListeners = [];
    private readonly List<EventDelegate<NavigationEvent>> AfterListeners = [];
    private readonly List<EventDelegate<NavigationEvent>> AfterFinishListeners = [];
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public Navigator() {
        if (!AppEnvironment.IsServer) {
            Manager = AppEnvironment.GetService<NavigationManager>();
            Manager.RegisterLocationChangingHandler(BeforeLocationChanged);
            Manager.LocationChanged += AfterLocationChanged;
        }
        Disposer = new(this, NavLock.Dispose);
    }
    private readonly Disposer Disposer;
    public void Dispose() => Disposer.Dispose();
    ~Navigator() => Disposer.Final();

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(Action<string, bool, bool> serverRedirect, Action serverRefresh) {
        if (ServerRedirect is not null) throw new InvalidOperationException("Navigator already initialized!");
        ServerRedirect = serverRedirect;
        ServerRefresh = serverRefresh;
    }

    public static void Init() => Init((url, forceLoad, replace) => {}, () => {});

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private async ValueTask BeforeLocationChanged(LocationChangingContext ctx) {
        if (ctx.IsNavigationIntercepted || IsPopState) await NavLock.TryLock();
        await PageLoader.Show(PAGE_LOADER_TASK.NAVIGATION, true);
        PreviousURL = URL.Url();

        foreach (var blocker in Blockers) {
            if (!await blocker.Invoke(new(PreviousURL, ctx.TargetLocation))) {
                ctx.PreventNavigation();
                await PageLoader.Hide(PAGE_LOADER_TASK.NAVIGATION, false);
                NavLock.TryUnlock();
                return;
            }
        }
        
        if (URL.IsLocal(ctx.TargetLocation) && !ProgramNavigation) {
            if (Page.Current.GetType() == typeof(Error404Page) && PreviousURL == ctx.TargetLocation) {
                ctx.PreventNavigation();
                await PageLoader.Hide(PAGE_LOADER_TASK.NAVIGATION, false);
                NavLock.TryUnlock();
                return;
            }
        }

        if (Loader) {
            await PageLoader.Show(PAGE_LOADER_TASK.NAVIGATION);
            MinLoadingWatch.Start();
        }

        foreach (var listener in BeforeListeners) {
            await listener.Invoke(new(PreviousURL, ctx.TargetLocation));
        }
    }

    private TaskCompletionSource PageRenderedTCS = new();
    protected static void PageRendered() => Instance().PageRenderedTCS.TrySetResult();
    public const string PAGE_RENDERED = nameof(PageRendered);

    private async void AfterLocationChanged (object? sender, LocationChangedEventArgs e) {
        if (Notify != NOTIFY.STATE && !SettingQueries) PageRenderedTCS = new();
        foreach (var listener in AfterListeners) {
            await listener.Invoke(new(PreviousURL, e.Location));
        }

        if (Notify is NOTIFY notify) {
            AppLayout.Notify(notify);
            NavigationFinished.TrySetResult();
        } else {
            var samePage = URL.PathMatches(URL.Path(PreviousURL), URL.Path(e.Location));
            if (ProgramNavigation) {
                if (!SettingQueries && URL.IsLocal(e.Location)) {
                    AppLayout.Notify(samePage ? NOTIFY.PAGE : NOTIFY.STATE);
                }
                NavigationFinished.TrySetResult();
            } else {
                AppLayout.Notify(samePage ? NOTIFY.PAGE : NOTIFY.STATE);
            }
        }

        if (NavState != null) SetState(NavState);

        if (Notify != NOTIFY.STATE && !SettingQueries) await PageRenderedTCS.Task;

        ProgramNavigation = false;
        SettingQueries = false;
        NavData = null;
        NavState = null;
        Notify = null;
        IsPopState = false;

        if (Loader) await MinLoadingWatch.Task;
        await PageLoader.Hide(PAGE_LOADER_TASK.NAVIGATION, false);
        Loader = true;
        
        foreach (var listener in AfterFinishListeners) {
            await listener.Invoke(new(PreviousURL, e.Location));
        }
        NavLock.TryUnlock();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private async Task Navigate(
        string url,
        bool forceLoad = false, bool replace = false, bool queries = false,
        object? data = null, object? state = null, NOTIFY? notify = null,
        bool loader = true
    ) {
        await NavLock.TryLock();

        if (AppEnvironment.IsServer) {
            ServerRedirect(url, forceLoad, replace);
            RequestStorage.Set(nameof(URL), url);
            NavLock.TryUnlock();
            return;
        }

        ProgramNavigation = true;
        SettingQueries = queries;
        Loader = loader;
        NavData = data;
        NavState = AppEnvironment.IsClient ? state : null;
        Notify = notify;
        NavigationFinished = new TaskCompletionSource();

        if (!queries && URL.IsLocal(url)) {
            if (!I18N.IsCurrentCultureUrl(url)) forceLoad = true;
        }

        Manager.NavigateTo(url, forceLoad, replace);
        await NavigationFinished.Task;
    }

    public static async Task NavigateTo(
        string url, bool forceLoad = false, bool replace = false,
        object? data = null, object? state = null, NOTIFY? notify = null
    ) => await Instance().Navigate(
        url,
        forceLoad, replace, queries: false,
        data, state, notify,
        loader: true
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
    public static T? State<T>() => AppEnvironment.IsClient ? JS.Invoke<T?>(JSNavigator.State) : default;
    public static T State<T>(T fallback) => AppEnvironment.IsClient ? JS.Invoke<T?>(JSNavigator.State) ?? fallback : fallback;
    public static void SetState<T>(T state, string? url = null) { if (AppEnvironment.IsClient) JS.InvokeVoid(JSNavigator.SetState, state, url); }

    // Pop state --------------------------------------------------------------------------------------------------------------------------
    public static bool IsPopState { get; private set; } = false;

    [JSInvokable]
    public static void JS_PopState() => IsPopState = true;

    // Listeners --------------------------------------------------------------------------------------------------------------------------
    public static async Task AddBlocker(EventPredicate<NavigationEvent> predicate) {
        var instance = Instance();
        await instance.NavLock.TryExclusive(() => instance.Blockers.Add(predicate));
    }
    public static async Task AddBlocker(Func<NavigationEvent, bool> predicate) {
        await AddBlocker(new EventPredicate<NavigationEvent>(predicate));
    }
    public static async Task AddBlocker(Func<NavigationEvent, Task<bool>> predicate) {
        await AddBlocker(new EventPredicate<NavigationEvent>(predicate));
    }

    public static async Task RemoveBlocker(EventPredicate<NavigationEvent> predicate) {
        var instance = Instance();
        await instance.NavLock.TryExclusive(() => {
            for (int i = 0; i < instance.Blockers.Count; i++) {
                if (!predicate.Equals(instance.Blockers[i])) continue;
                instance.Blockers.RemoveAt(i); break;
            }
        });
    }
    public static async Task RemoveBlocker(Func<NavigationEvent, bool> predicate) {
        await RemoveBlocker(new EventPredicate<NavigationEvent>(predicate));
    }
    public static async Task RemoveBlocker(Func<NavigationEvent, Task<bool>> predicate) {
        await RemoveBlocker(new EventPredicate<NavigationEvent>(predicate));
    }

    public static async Task AddBeforeEventListener(EventDelegate<NavigationEvent> listener) {
        var instance = Instance();
        await instance.NavLock.TryExclusive(() => instance.BeforeListeners.Add(listener));
    }
    public static async Task AddBeforeEventListener(Action<NavigationEvent> listener) {
        await AddBeforeEventListener(new EventDelegate<NavigationEvent>(listener));
    }
    public static async Task AddBeforeEventListener(Func<NavigationEvent, Task> listener) {
        await AddBeforeEventListener(new EventDelegate<NavigationEvent>(listener));
    }

    public static async Task RemoveBeforeEventListener(EventDelegate<NavigationEvent> listener) {
        var instance = Instance();
        await instance.NavLock.TryExclusive(() => {
            for (int i = 0; i < instance.BeforeListeners.Count; i++) {
                if (!listener.Equals(instance.BeforeListeners[i])) continue;
                instance.BeforeListeners.RemoveAt(i); break;
            }
        });
    }
    public static async Task RemoveBeforeEventListener(Action<NavigationEvent> listener) {
        await RemoveBeforeEventListener(new EventDelegate<NavigationEvent>(listener));
    }
    public static async Task RemoveBeforeEventListener(Func<NavigationEvent, Task> listener) {
        await RemoveBeforeEventListener(new EventDelegate<NavigationEvent>(listener));
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
