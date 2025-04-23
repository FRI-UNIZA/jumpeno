namespace Jumpeno.Client.Services;

using System.Diagnostics;

#pragma warning disable CS8618
#pragma warning disable CA1816

public class Navigator : StaticService<Navigator>, IDisposable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly NavigationManager Manager;
    private static Action<string, bool, bool> ServerRedirect;
    private static Action ServerRefresh;
    private bool ProgramNavigation;
    private bool SettingQueries;
    private bool Loader = true;
    private string PreviousURL;
    private TaskCompletionSource NavigationFinished;
    private const int MIN_LOADING_MS = 100;
    private Stopwatch Watch;
    private readonly LockerSlim NavLock = new();

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
        ProgramNavigation = false;
        SettingQueries = false;
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

    public static void Init() {
        Init((string url, bool forceLoad, bool replace) => {}, () => {});
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private async ValueTask BeforeLocationChanged(LocationChangingContext ctx) {
        if (!ProgramNavigation) await NavLock.TryLock();
        await PageLoader.Show(PAGE_LOADER_TASK.NAVIGATION, true);
        PreviousURL = URL.Url();

        foreach (var blocker in Blockers) {
            if (!await blocker.Invoke(new(PreviousURL, ctx.TargetLocation))) {
                ctx.PreventNavigation();
                await PageLoader.Hide(PAGE_LOADER_TASK.NAVIGATION);
                NavLock.TryUnlock();
                return;
            }
        }
        
        if (URL.IsLocal(ctx.TargetLocation)) {
            if (!ProgramNavigation) {
                var isSameURL = URL.Path(PreviousURL) == URL.Path(ctx.TargetLocation);
                if (Page.Current.GetType() == typeof(Error404Page)) {
                    ctx.PreventNavigation();
                    NavLock.TryUnlock();
                    await NavigateTo(ctx.TargetLocation);
                    return;
                }
                
                if (isSameURL && IsPopState) {
                    ctx.PreventNavigation();
                    NavLock.TryUnlock();
                    await NavigateTo(ctx.TargetLocation, forceLoad: true, replace: true);
                    return;
                }

                var isCurrentCulture = I18N.IsCurrentCultureUrl(ctx.TargetLocation);
                if (isSameURL || !isCurrentCulture) {
                    ctx.PreventNavigation();
                    NavLock.TryUnlock();
                    await NavigateTo(ctx.TargetLocation, replace: isCurrentCulture);
                    return;
                }
            }
        }

        if (Loader) {
            await PageLoader.Show(PAGE_LOADER_TASK.NAVIGATION);
            Watch = Stopwatch.StartNew();
        }

        foreach (var listener in BeforeListeners) {
            await listener.Invoke(new(PreviousURL, ctx.TargetLocation));
        }
    }

    private async void AfterLocationChanged (object? sender, LocationChangedEventArgs e) {
        foreach (var listener in AfterListeners) {
            await listener.Invoke(new(PreviousURL, e.Location));
        }

        if (ProgramNavigation) {
            if (!SettingQueries && URL.IsLocal(e.Location)) {
                LayoutBase.Current.Notify();
            }
            NavigationFinished.TrySetResult();
        }

        if (Loader) {
            Watch?.Stop();
            if ((Watch?.ElapsedMilliseconds ?? 0) > MIN_LOADING_MS) {
                await PageLoader.Hide(PAGE_LOADER_TASK.NAVIGATION);
            } else {
                await Task.Delay(MIN_LOADING_MS - (int) (Watch?.ElapsedMilliseconds ?? 0));
                await PageLoader.Hide(PAGE_LOADER_TASK.NAVIGATION);
            }
        } else await PageLoader.Hide(PAGE_LOADER_TASK.NAVIGATION);

        ProgramNavigation = false;
        SettingQueries = false;
        Loader = true;
        IsPopState = false;
        
        foreach (var listener in AfterFinishListeners) {
            await listener.Invoke(new(PreviousURL, e.Location));
        }
        NavLock.TryUnlock();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private async Task Navigate(string url, bool forceLoad = false, bool replace = false, bool queries = false, bool loader = true) {
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
        NavigationFinished = new TaskCompletionSource();

        if (!queries && URL.IsLocal(url)) {
            if (!I18N.IsCurrentCultureUrl(url) || Page.Current.GetType() == typeof(Error404Page)) {
                forceLoad = true;
            }
            if (URL.Path(url) == URL.Path()) {
                replace = true;
            }
        }

        Manager.NavigateTo(url, forceLoad, replace);
        await NavigationFinished.Task;
    }

    public static async Task NavigateTo(string url, bool forceLoad = false, bool replace = false) {
        await Instance().Navigate(url, forceLoad, replace, queries: false, true);
    }
    public static void Refresh() {
        if (AppEnvironment.IsServer) ServerRefresh();
        else Instance().Manager.Refresh(forceReload: true);
    }
    public static async Task SetQueryParams(QueryParams queryParams) {
        await Instance().Navigate(URL.SetQueryParams(queryParams), forceLoad: false, replace: true, queries: true, loader: false);
    }

    // State ------------------------------------------------------------------------------------------------------------------------------
    public static T State<T>() => JS.Invoke<T>(JSNavigator.State);
    public static void SetState<T>(T state, string? url = null) => JS.InvokeVoid(JSNavigator.SetState, state, url);

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
