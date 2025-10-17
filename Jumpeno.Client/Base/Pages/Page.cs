namespace Jumpeno.Client.Base;

public class Page : ComponentBase, IAsyncDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Used to specify language specific URL:
    public const string ROUTE_PREFIX = "ROUTE";
    // Used to determine role page access:
    public const string ROLES_NAME = "ROLES";
    public const string ROLES_BLOCK_NAME = "ROLES_BLOCK";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter(Name = ThemeProvider.CASCADE_APP_THEME)]
    public BaseTheme AppTheme { get; set; } = null!;

    // Current page -----------------------------------------------------------------------------------------------------------------------
    public static Page Current => RequestStorage.Get<Page>(REQUEST_STORAGE.PAGE) ?? new Error404Page();
    private static void SetCurrent(Page page) => RequestStorage.Set(REQUEST_STORAGE.PAGE, page);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public long ComponentCount { get; private set; } = 0;
    public void CountComponent() => ComponentCount++;
    /// <summary>Returns all page urls as an array.</summary>
    /// <returns>Array of URLs</returns>
    public string[] GetPageUrls() => GetType().GetCustomAttributes<RouteAttribute>().Select(x => x.Template).ToArray();
    // Dispose:
    public bool IsDisposing { get; private set; } = false;
    public bool IsDisposed { get; private set; } = false;

    // Types ------------------------------------------------------------------------------------------------------------------------------
    private static readonly IEnumerable<Type> PageTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Page)));
    public static Type? Type(RenderFragment? body) {
        try { return (body?.Target as RouteView)?.RouteData?.PageType; }
        catch { return null; }
    }
    public static Type? Type(string noSegmentPath) {
        return PageTypes.FirstOrDefault(t => {
            try {
                foreach (var lang in I18N.LANGUAGES) {
                    string link = t.GetField($"{ROUTE_PREFIX}_{lang.ToUpper()}")!.GetValue(null)!.ToString()!;
                    link = URL.RemoveSegments(link);
                    if (link.Equals(noSegmentPath, StringComparison.CurrentCultureIgnoreCase)) return true;
                }
                return false;
            } catch {
                return false;
            }
        });
    }

    // Roles ------------------------------------------------------------------------------------------------------------------------------
    public static ROLE[] Roles(Type? type) {
        if (type == null) return [];
        var attr = type.GetField(ROLES_NAME);
        if (attr == null) return [];
        ROLE[]? roles = (ROLE[]?) attr.GetValue(null);
        return roles ?? [];
    }
    public static ROLE[] Roles(RenderFragment? body) {
        if (body == null) return [];
        return Roles(Type(body));
    }
    public static ROLE[] Roles(string url) => Roles(Type(url));

    public static ROLE[] RolesBlock(Type? type) {
        if (type == null) return [];
        var attr = type.GetField(ROLES_BLOCK_NAME);
        if (attr == null) return [];
        ROLE[]? roles = (ROLE[]?) attr.GetValue(null);
        return roles ?? [];
    }
    public static ROLE[] RolesBlock(RenderFragment? body) {
        if (body == null) return [];
        return RolesBlock(Type(body));
    }
    public static ROLE[] RolesBlock(string url) => RolesBlock(Type(url));

    // Useful methods ---------------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Method is called when language is switched.
    ///     New language url is obtained and passed as parameter.
    ///     Can be used to improve automatically determined url.
    /// </summary>
    /// <param name="culture">Switched language culture.</param>
    /// <param name="url">Automaticall determined page url for new culture.</param>
    /// <returns>Improved url after language is switched.</returns>
    /// <exception cref="Exception">Thrown when new URL can not be determined and language switch should not happen.</exception>
    public virtual string CustomURL(string culture, string url) => url;

    /// <summary>
    ///     Method is called when creating link to the page with parameters set as path segments.
    ///     This is only an example to set path parameters for url "/path/{id}/path-continues/{name}".
    ///     In order for page link to have parameters, static method in custom page has to be implemented.
    /// </summary>
    /// <param name="url">URL without segment parameters set.</param>
    /// <param name="id">Example of id as the first path segment parameter.</param>
    /// <param name="name">Example of name as the third path segment parameter.</param>
    /// <returns></returns>
    public static string Link(string url, int id, string name) => URL.ReplaceSegments(
        url,
        new Dictionary<int, string> {
            { 1, $"{id}" },
            { 3, name }
        }
    );

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override sealed void OnInitialized() {
        SetCurrent(this);
        OnPageInitialized();
        if (AppEnvironment.IsServer) return;
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }
    protected override sealed async Task OnInitializedAsync() => await OnPageInitializedAsync();
    private bool ParametersSet = false;
    protected sealed override void OnParametersSet() {
        OnPageParametersSet(!ParametersSet);
        ParametersSet = true;
    }
    private bool ParametersSetAsync = false;
    protected sealed override async Task OnParametersSetAsync() {
        var firstTime = !ParametersSetAsync;
        ParametersSetAsync = true;
        await OnPageParametersSetAsync(firstTime);
    }
    protected sealed override bool ShouldRender() => ShouldPageRender();
    protected sealed override void OnAfterRender(bool firstRender) => OnPageAfterRender(firstRender);
    protected sealed override async Task OnAfterRenderAsync(bool firstRender) {
        await OnPageAfterRenderAsync(firstRender);
        if (!firstRender) return;
        if (Auth.Processing) return;
        Reflex.InvokeVoid(typeof(Navigator), Navigator.PAGE_RENDERED);
    }
    public async ValueTask DisposeAsync() {
        IsDisposing = true;
        OnPageDispose();
        await OnPageDisposeAsync();
        GC.SuppressFinalize(this);
        IsDisposed = true;
    }

    // Lifecycle overrides ----------------------------------------------------------------------------------------------------------------
    protected virtual void OnPageInitialized() {}
    protected virtual Task OnPageInitializedAsync() => Task.CompletedTask;
    protected virtual void OnPageParametersSet(bool firstTime) {}
    protected virtual Task OnPageParametersSetAsync(bool firstTime) => Task.CompletedTask;
    protected virtual bool ShouldPageRender() => true;
    protected virtual void OnPageAfterRender(bool firstRender) {}
    protected virtual Task OnPageAfterRenderAsync(bool firstRender) => Task.CompletedTask;
    protected virtual void OnPageDispose() {}
    protected virtual ValueTask OnPageDisposeAsync() => ValueTask.CompletedTask;

    // Notification -----------------------------------------------------------------------------------------------------------------------
    public void Notify() => StateHasChanged();
}
