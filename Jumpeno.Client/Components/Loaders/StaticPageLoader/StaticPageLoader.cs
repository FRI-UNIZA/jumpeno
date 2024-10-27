namespace Jumpeno.Client.Components;

public partial class StaticPageLoader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_LOADER = "static-page-loader";
    public const string CLASS_TITLE = "static-page-loader-title";
    public const string CLASS_HIDDEN = "hidden";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool Hidden = AppSettings.Prerender;
    private bool Displayed = !AppSettings.Prerender;
    private TaskCompletionSource HideTCS = null!;
    public CSSClass ComputeClass() {
        var c = new CSSClass(ID_LOADER);
        if (Hidden) c.Set(CLASS_HIDDEN); 
        return c;
    }
    public int ComputeTabIndex() {
        return Hidden ? -1 : 0;
    }

    private static StaticPageLoader Instance = null!;

    public StaticPageLoader() {
        if (AppEnvironment.IsServer()) return;
        Instance = this;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnAfterRender(bool firstRender) {
        if (!firstRender) return;
        JS.InvokeVoid(JSStaticPageLoader.Activate, ID_LOADER);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task Hide() {
        if (AppSettings.Prerender || AppEnvironment.IsServer()) return;
        Instance.Hidden = true;
        Instance.HideTCS = new TaskCompletionSource();
        Instance.StateHasChanged();
        await Instance.HideTCS.Task;
        Instance.Displayed = false;
        Instance.StateHasChanged();
    }

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public static void JS_LoaderAnimated() {
        Instance.HideTCS.TrySetResult();
    }
}
