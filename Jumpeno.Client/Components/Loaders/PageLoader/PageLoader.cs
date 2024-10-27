namespace Jumpeno.Client.Components;

public partial class PageLoader {
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
    private readonly SemaphoreSlim Lock = new(1, 1);
    private readonly Dictionary<PAGE_LOADER_TASK, bool> PageLoaderTasks = new() { { PAGE_LOADER_TASK.INITIAL, true } };
    private readonly Dictionary<PAGE_LOADER_TASK, bool> GlobalLoaders = AppSettings.Prerender ? new() { { PAGE_LOADER_TASK.INITIAL, true } } : [];
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
        var instance = Instance();
        await instance.Lock.WaitAsync();
        instance.PageLoaderTasks[task] = true;
        
        instance.PageLoaderDisplayed = true;
        instance.UpdateGlobalLoaders(task, custom);
        if (!AppEnvironment.IsServer()) {
            var firstLoader = instance.PageLoaderTasks.Count == 1;
            if (firstLoader) {
                ActionHandler.SaveActiveElement();
                instance.NoLoaderTCS = new TaskCompletionSource();
            }
            await instance.OnChange();
            if (firstLoader) ActionHandler.SetFocus(ID);
        }

        instance.Lock.Release();
    }

    public static async Task Hide(PAGE_LOADER_TASK task = PAGE_LOADER_TASK.DEFAULT) {
        var instance = Instance();
        await instance.Lock.WaitAsync();
        instance.PageLoaderTasks.Remove(task);

        if (instance.PageLoaderDisplayed) {
            instance.PageLoaderDisplayed = instance.PageLoaderTasks.Count > 0;
            instance.RemoveGlobalLoader(task);
            if (!AppEnvironment.IsServer()) {
                await instance.OnChange();
                if (!instance.PageLoaderDisplayed) {
                    instance.NoLoaderTCS.TrySetResult();
                    await ActionHandler.RestoreFocusAsync();
                }
            }
        }

        instance.Lock.Release();
    }

    private static async Task WithActiveLoader(EmptyDelegate callback) {
        var instance = Instance();
        await instance.Lock.WaitAsync();
        if (instance.PageLoaderTasks.Count > 0) {
            await callback.Invoke();
        }
        instance.Lock.Release();
    }
    public static async Task WithActiveLoader(Action callback) {
        await WithActiveLoader(new EmptyDelegate(callback));
    }
    public static async Task WithActiveLoader(Func<Task> callback) {
        await WithActiveLoader(new EmptyDelegate(callback));
    }

    public static async Task AwaitAllLoaders() {
        if (AppEnvironment.IsServer()) return;
        var instance = Instance();
        await instance.Lock.WaitAsync();
        if (instance.PageLoaderTasks.Count > 0) {
            var tcs = instance.NoLoaderTCS.Task;
            instance.Lock.Release();
            await tcs;
        } else {
            instance.Lock.Release();
        }
    }

    public static async Task<bool> IsActiveTask(PAGE_LOADER_TASK task) {
        var instance = Instance();
        await instance.Lock.WaitAsync();
        var result = Instance().PageLoaderTasks.ContainsKey(task);
        instance.Lock.Release();
        return result;
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
