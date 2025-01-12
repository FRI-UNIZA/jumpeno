namespace Jumpeno.Client.Components;

public partial class PageLoader : IDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_CONTENT = "page-loader-content";
    public const string ID = "page-loader";
    public const string CLASS_BEFORE = "page-loader-before";
    public const string CLASS_AFTER = "page-loader-after";
    public const string CLASS_DISPLAYED = "displayed";
    public const string CLASS_CUSTOM = "custom-loader";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool PageLoaderDisplayed { get; set; } = true;
    private readonly LockerSlim Lock = new();
    private readonly Dictionary<PAGE_LOADER_TASK, bool> PageLoaderTasks = new() {{ PAGE_LOADER_TASK.INITIAL, true }};
    private readonly Dictionary<PAGE_LOADER_TASK, bool> GlobalLoaders = AppSettings.Prerender ? new() {{ PAGE_LOADER_TASK.INITIAL, true }} : [];
    private TaskCompletionSource NoLoaderTCS = new();
    private TaskCompletionSource RenderTCS = null!;
    public CSSClass ComputeClassContent() {
        var c = new CSSClass(CLASS_CONTENT);
        c.Set(ThemeProvider.CLASSNAME_THEME_TRANSITION_CONTAINER);
        return c;
    }
    public CSSClass ComputeClass() {
        var c = new CSSClass(ID);
        if (PageLoaderDisplayed) c.Set(CLASS_DISPLAYED);
        if (GlobalLoaders.Count == 0) c.Set(CLASS_CUSTOM);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        Lock.Dispose();
        GC.SuppressFinalize(this);
    }

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
            instance.PageLoaderTasks[task] = true;
            // Focus handling:
            instance.PageLoaderDisplayed = true;
            instance.UpdateGlobalLoaders(task, custom);
            if (!AppEnvironment.IsServer) {
                var firstLoader = instance.PageLoaderTasks.Count == 1;
                if (firstLoader) {
                    ActionHandler.SaveActiveElement();
                    instance.NoLoaderTCS = new TaskCompletionSource();
                }
                await instance.OnChange();
                if (firstLoader) ActionHandler.SetFocus(ID);
            }
        });
    }

    public static async Task Hide(PAGE_LOADER_TASK task = PAGE_LOADER_TASK.DEFAULT) {
        var instance = Instance(); await instance.Lock.TryExclusive(async () => {
            instance.PageLoaderTasks.Remove(task);
            // Focus handling:
            if (instance.PageLoaderDisplayed) {
                instance.PageLoaderDisplayed = instance.PageLoaderTasks.Count > 0;
                instance.RemoveGlobalLoader(task);
                if (!AppEnvironment.IsServer) {
                    await instance.OnChange();
                    if (!instance.PageLoaderDisplayed) {
                        instance.NoLoaderTCS.TrySetResult();
                        await ActionHandler.RestoreFocusAsync();
                    }
                }
            }
        });
    }

    public static async Task Show(Func<Task> action, PAGE_LOADER_TASK task = PAGE_LOADER_TASK.DEFAULT) {
        await Show(task);
        try { await action(); }
        finally { await Hide(task); }
    }

    public static async Task Try(Func<Task> action, PAGE_LOADER_TASK task = PAGE_LOADER_TASK.DEFAULT) {
        try { await Show(action, task); }
        catch {}
    }

    private static async Task WithActiveLoader(EmptyDelegate callback) {
        var instance = Instance(); await instance.Lock.TryExclusive(async () => {
            if (instance.PageLoaderTasks.Count <= 0) return;
            await callback.Invoke();
        });
    }
    public static async Task WithActiveLoader(Action callback) {
        await WithActiveLoader(new EmptyDelegate(callback));
    }
    public static async Task WithActiveLoader(Func<Task> callback) {
        await WithActiveLoader(new EmptyDelegate(callback));
    }

    public static async Task AwaitAllLoaders() {
        if (AppEnvironment.IsServer) return;
        var instance = Instance(); await instance.Lock.TryExclusive(async token => {
            if (instance.PageLoaderTasks.Count <= 0) return;
            var tcs = instance.NoLoaderTCS.Task;
            token.Unlock();
            await tcs;
        });
    }

    public static async Task<bool> IsActiveTask(PAGE_LOADER_TASK task) {
        var instance = Instance(); return await instance.Lock.TryExclusive(() => {
            return instance.PageLoaderTasks.ContainsKey(task);
        }, false);
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static async void JS_Show() {
        await Show();
    }

    [JSInvokable]
    public static void JS_AfterAnimationFrame() {
        Instance().RenderTCS.TrySetResult();
    }
}
