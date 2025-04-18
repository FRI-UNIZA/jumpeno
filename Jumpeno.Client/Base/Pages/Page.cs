namespace Jumpeno.Client.Base;

using System.Reflection;

public class Page : ComponentBase, IAsyncDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    // NOTE: Used to trigger page rerender on theme change! (Important for Image URL)
    private BaseTheme? RenderTheme { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public long ComponentCount { get; private set; } = 0;
    public void CountComponent() => ComponentCount++;
    /// <summary>
    ///     Returns all page urls as an array.
    /// </summary>
    /// <returns>Array of URLs</returns>
    public string[] GetPageUrls() => GetType().GetCustomAttributes<RouteAttribute>().Select(x => x.Template).ToArray();

    // Static methods ---------------------------------------------------------------------------------------------------------------------
    public static Page Current => RequestStorage.Get<Page>(nameof(Page)) ?? new Error404Page();
    private static void SetCurrent(Page page) => RequestStorage.Set(nameof(Page), page);

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
        if (!PageAuth.CanRender()) return;
        if (!PageAuth.IsAuthorized(this)) throw Exceptions.NotAuthorized;
        OnPageInitialized();
        if (AppEnvironment.IsServer) return;
        ScrollArea.ScrollTo(SCROLLAREA_ID.PAGE, 0, 0);
    }

    protected override sealed async Task OnInitializedAsync() {
        if (!PageAuth.CanRender()) return;
        await OnPageInitializedAsync();
    }
    private bool ParametersSet = false;
    protected sealed override void OnParametersSet() {
        if (!PageAuth.CanRender()) return;
        OnPageParametersSet(!ParametersSet);
        ParametersSet = true;
    }
    private bool ParametersSetAsync = false;
    protected sealed override async Task OnParametersSetAsync() {
        if (!PageAuth.CanRender()) return;
        var firstTime = !ParametersSetAsync;
        ParametersSetAsync = true;
        await OnPageParametersSetAsync(firstTime);
    }
    protected sealed override void OnAfterRender(bool firstRender) {
        if (!PageAuth.CanRender()) return;
        OnPageAfterRender(firstRender);
    }
    protected sealed override async Task OnAfterRenderAsync(bool firstRender) {
        if (!PageAuth.CanRender()) return;
        await OnPageAfterRenderAsync(firstRender);
    }
    public async ValueTask DisposeAsync() {
        if (!PageAuth.CanRender()) return;
        OnPageDispose();
        await OnPageDisposeAsync();
        await HTTP.ClearTokens();
        GC.SuppressFinalize(this);
    }

    // Lifecycle overrides ----------------------------------------------------------------------------------------------------------------
    protected virtual void OnPageInitialized() {}
    protected virtual Task OnPageInitializedAsync() => Task.CompletedTask;
    protected virtual void OnPageParametersSet(bool firstTime) {}
    protected virtual Task OnPageParametersSetAsync(bool firstTime) => Task.CompletedTask;
    protected virtual void OnPageAfterRender(bool firstRender) {}
    protected virtual Task OnPageAfterRenderAsync(bool firstRender) => Task.CompletedTask;
    protected virtual void OnPageDispose() {}
    protected virtual ValueTask OnPageDisposeAsync() => ValueTask.CompletedTask;
}
