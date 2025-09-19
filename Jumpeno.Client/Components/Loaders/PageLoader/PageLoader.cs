namespace Jumpeno.Client.Components;

public partial class PageLoader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "page-loader";
    // Class:
    public const string CLASS_CONTENT = "page-loader-content";
    public const string CLASS_BEFORE = "page-loader-before";
    public const string CLASS_AFTER = "page-loader-after";
    public const string CLASS_DISPLAYED = "displayed";
    public const string CLASS_CUSTOM = "custom-loader";
    // Cascade:
    public const string CASCADE_PAGE_LOADER_DISPLAYED = $"{nameof(PageLoader)}.{nameof(CASCADE_PAGE_LOADER_DISPLAYED)}";
    // Min loading time:
    public const uint MIN_LOADING = 150; // ms

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public PAGE_LOADER_SURFACE? Surface { get; set; } = PAGE_LOADER_SURFACE.SECONDARY;
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool PageLoaderDisplayed { get; set; } = true;
    private readonly LockerSlim Lock = new();
    private readonly Dictionary<PAGE_LOADER_TASK, bool> PageLoaderTasks = new() {{ PAGE_LOADER_TASK.INITIAL, true }};
    private readonly Dictionary<PAGE_LOADER_TASK, bool> GlobalLoaders = [];
    private readonly MinWatch MinWatch = new(MIN_LOADING);
    private TaskCompletionSource NoLoaderTCS = new();
    private TaskCompletionSource RenderTCS = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public static CSSClass ComputeContentClass() => new CSSClass(CLASS_CONTENT).Set(ThemeProvider.CLASS_THEME_TRANSITION_CONTAINER);

    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(ID, Base)
        .SetSurface(Surface)
        .Set(CLASS_DISPLAYED, PageLoaderDisplayed)
        .Set(CLASS_CUSTOM, GlobalLoaders.Count == 0);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentDispose() => Lock.Dispose();

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private async Task OnChange() {
        StateHasChanged();
        RenderTCS = new TaskCompletionSource();
        JS.InvokeVoid(JSPageLoader.RequestAnimationFrame);
        await RenderTCS.Task;
    }

    private bool UpdateGlobalLoaders(PAGE_LOADER_TASK task, bool custom) {
        if (custom) return false;
        GlobalLoaders[task] = true;
        return GlobalLoaders.Count == 1;
    }
    private bool RemoveGlobalLoader(PAGE_LOADER_TASK task) {
        var custom = !GlobalLoaders.Remove(task);
        if (custom) return false;
        return GlobalLoaders.Count == 0;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task Show(PAGE_LOADER_TASK task = PAGE_LOADER_TASK.DEFAULT, bool custom = false) {
        var instance = Instance(); await instance.Lock.TryExclusive(async () => {
            // 1) Set tasks:
            instance.PageLoaderTasks[task] = true;
            instance.UpdateGlobalLoaders(task, custom);
            // 2) Set loading:
            var firstLoader = !instance.PageLoaderDisplayed;
            instance.PageLoaderDisplayed = true;
            // 3) Client actions:
            if (AppEnvironment.IsClient) {
                if (firstLoader) {
                    instance.MinWatch.Start();
                    ActionHandler.DisableKeyboardActions();
                    ActionHandler.SaveActiveElement();
                    instance.NoLoaderTCS = new TaskCompletionSource();
                }
                await instance.OnChange();
                if (firstLoader) {
                    ActionHandler.SetFocus(ID);
                    Window.Inert();
                }
            }
        });
    }

    public static async Task Hide(PAGE_LOADER_TASK task = PAGE_LOADER_TASK.DEFAULT, bool minLoading = true) {
        var instance = Instance(); await instance.Lock.TryExclusive(async () => {
            if (!instance.PageLoaderDisplayed) return;
            // 1) Remove tasks:
            instance.PageLoaderTasks.Remove(task);
            instance.RemoveGlobalLoader(task);
            // 2) Check loading:
            var lastLoader = instance.PageLoaderTasks.Count <= 0;
            // 3) Update state:
            if (AppEnvironment.IsClient) {
                if (lastLoader) {
                    if (minLoading) await instance.MinWatch.Task;
                    instance.PageLoaderDisplayed = false;
                }
                await instance.OnChange();
                if (lastLoader) {
                    instance.NoLoaderTCS.TrySetResult();
                    ActionHandler.EnableKeyboardActions();
                    await ActionHandler.RestoreFocusAsync();
                    Window.Inert();
                }
            } else {
                instance.PageLoaderDisplayed = !lastLoader;
            }
        });
    }

    public static async Task Show(
        Func<Task> action, PAGE_LOADER_TASK task = PAGE_LOADER_TASK.DEFAULT,
        bool custom = false, bool minLoading = true
    ) {
        await Show(task, custom);
        try { await action(); }
        finally { await Hide(task, minLoading); }
    }

    public static async Task Try(
        Func<Task> action, PAGE_LOADER_TASK task = PAGE_LOADER_TASK.DEFAULT,
        bool custom = false, bool minLoading = true
    ) {
        try { await Show(action, task, custom, minLoading); }
        catch {}
    }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> IsActive() {
        var instance = Instance(); return await instance.Lock.TryExclusive(() => {
            return instance.PageLoaderDisplayed;
        }, false);
    }

    public static async Task<bool> IsActiveTask(PAGE_LOADER_TASK task) {
        var instance = Instance(); return await instance.Lock.TryExclusive(() => {
            return instance.PageLoaderTasks.ContainsKey(task);
        }, false);
    }

    // Conditionals -----------------------------------------------------------------------------------------------------------------------
    private static async Task WithActiveLoader(EmptyDelegate callback) {
        var instance = Instance(); await instance.Lock.TryExclusive(async () => {
            if (!instance.PageLoaderDisplayed) return;
            await callback.Invoke();
        });
    }
    public static async Task WithActiveLoader(Action callback) => await WithActiveLoader(new EmptyDelegate(callback));
    public static async Task WithActiveLoader(Func<Task> callback) => await WithActiveLoader(new EmptyDelegate(callback));

    // Await ------------------------------------------------------------------------------------------------------------------------------
    public static async Task AwaitAllLoaders() {
        if (AppEnvironment.IsServer) return;
        var instance = Instance(); await instance.Lock.TryExclusive(async token => {
            if (instance.PageLoaderTasks.Count <= 0) return;
            var tcs = instance.NoLoaderTCS.Task;
            token.Unlock();
            await tcs;
        });
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static async void JS_Show() => await Show();

    [JSInvokable]
    public static void JS_AfterAnimationFrame() => Instance().RenderTCS.TrySetResult();
}
