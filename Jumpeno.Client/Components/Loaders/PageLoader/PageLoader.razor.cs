namespace Jumpeno.Client.Components;

public partial class PageLoader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string Id = "page-loader";
    // Class:
    public const string ClassContent = "page-loader-content";
    public const string ClassBefore = "page-loader-before";
    public const string ClassAfter = "page-loader-after";
    public const string ClassDisplayed = "displayed";
    public const string ClassCustom = "custom-loader";
    // Cascade:
    public const string CascadePageLoaderDisplayed = $"{nameof(PageLoader)}.{nameof(CascadePageLoaderDisplayed)}";
    // Min loading time:
    public const uint MinLoading = 150; // ms

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public PageLoaderSurface? Surface { get; set; } = PageLoaderSurface.Secondary;
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool PageLoaderDisplayed { get; set; } = true;
    private readonly LockerSlim Lock = new();
    private readonly HashSet<PageLoaderTask> PageLoaderTasks = [PageLoaderTask.Initial];
    private readonly HashSet<PageLoaderTask> GlobalLoaders = [];
    private readonly MinWatch MinWatch = new(MinLoading);
    private TaskCompletionSource NoLoaderTCS = new();
    private TaskCompletionSource RenderTCS = null!;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public static CssClass ComputeContentClass() => new CssClass(ClassContent).Set(ThemeProvider.ClassThemeTransitionContainer);

    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(Id, Base)
        .SetSurface(Surface)
        .Set(ClassDisplayed, PageLoaderDisplayed)
        .Set(ClassCustom, GlobalLoaders.Count == 0);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async ValueTask OnComponentDisposeAsync() => await Lock.DisposeSafe();

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private bool UpdateGlobalLoaders(PageLoaderTask task, bool custom) {
        if (custom) return false;
        GlobalLoaders.Add(task);
        return GlobalLoaders.Count == 1;
    }
    private bool RemoveGlobalLoader(PageLoaderTask task) {
        var custom = !GlobalLoaders.Remove(task);
        if (custom) return false;
        return GlobalLoaders.Count == 0;
    }

    private async Task Render() {
        StateHasChanged();
        RenderTCS = new TaskCompletionSource();
        await Task.Yield();
        JS.InvokeVoid(JSPageLoader.RequestAnimationFrame);
        await RenderTCS.Task;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    /// <summary>Shows the page loader.</summary>
    /// <param name="task">Task to show page loader for</param>
    /// <param name="custom">True means invisible to only block user input (we can show custom loader)</param>
    /// <returns>Task to await</returns>
    public static async Task Show(PageLoaderTask task = PageLoaderTask.Default, bool custom = false) {
        var instance = Instance(); await instance.Lock.TryExclusive(async () => {
            // 1) Set tasks:
            instance.PageLoaderTasks.Add(task);
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
                await instance.Render();
                if (firstLoader) {
                    ActionHandler.SetFocus(Id);
                    Window.Inert();
                }
            }
        });
    }

    /// <summary>Hides the page loader.</summary>
    /// <param name="task">Task to hide page loader for</param>
    /// <param name="minLoading">True ensures that the page loader was displayed for at least the minimum loading time</param>
    /// <returns>True if page loader is hidden (no more active loaders)</returns>
    public static async Task<bool> Hide(PageLoaderTask task = PageLoaderTask.Default, bool minLoading = true) {
        var instance = Instance(); return await instance.Lock.TryExclusive(async () => {
            // 0) Check state:
            if (!instance.PageLoaderDisplayed) return true;
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
                await instance.Render();
                if (lastLoader) {
                    instance.NoLoaderTCS.TrySetResult();
                    ActionHandler.EnableKeyboardActions();
                    await ActionHandler.RestoreFocusAsync();
                    Window.Inert();
                }
            } else {
                instance.PageLoaderDisplayed = !lastLoader;
            }
            // 4) Return state:
            return !instance.PageLoaderDisplayed;
        });
    }

    public static async Task Show(
        Func<Task> action, PageLoaderTask task = PageLoaderTask.Default,
        bool custom = false, bool minLoading = true
    ) {
        await Show(task, custom);
        try { await action(); }
        finally { await Hide(task, minLoading); }
    }

    public static async Task Try(
        Func<Task> action, PageLoaderTask task = PageLoaderTask.Default,
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

    public static async Task<bool> IsActiveTask(PageLoaderTask task) {
        var instance = Instance(); return await instance.Lock.TryExclusive(() => {
            return instance.PageLoaderTasks.Contains(task);
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
    public static void JS_AfterAnimationFrame() => Instance().RenderTCS.TrySetResult();
}
