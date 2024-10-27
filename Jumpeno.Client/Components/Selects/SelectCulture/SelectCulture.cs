namespace Jumpeno.Client.Components;

public partial class SelectCulture {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "select-culture";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly List<SelectOption> Options = I18N.LANGUAGES.Select(x => new SelectOption(x, x.ToUpper())).ToList();
    private readonly SelectOption DefaultValue = new(I18N.Culture(), I18N.Culture().ToUpper());

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void Init() {
        var cookie = CookieStorage.Get(COOKIE_FUNCTIONAL.APP_CULTURE);
        if (cookie is null) return;
        CookieStorage.Set(new Cookie(
            COOKIE_FUNCTIONAL.APP_CULTURE,
            cookie,
            DateTimeOffset.Now.AddYears(1)
        ));
    }

    private static async Task OnSelect(SelectEvent ev) {
        await PageLoader.Show(PAGE_LOADER_TASK.CULTURE_CHANGE, !AppSettings.Prerender);
        if (!AppSettings.Prerender) {
            await ChangeCulture(ev);
        }
    }

    private static async Task OnCloseSelected(SelectEvent ev) {
        if (AppSettings.Prerender) {
            await ChangeCulture(ev);
        }
    }

    private static async Task ChangeCulture(SelectEvent ev) {
        var value = ev.After.GetValue<string>();

        if (I18N.Culture() == value) return;

        // Access info:
        var path = URL.Path();

        // Update uri with parameters:
        var page = Page.CurrentPage();
        if (page is not ErrorPage) {
            var pageURI = URL.Encode(page.GetType().GetField($"ROUTE_{value.ToString().ToUpper()}")!.GetValue(null)!.ToString()!);

            var currentSegments = $"{path}/".Split('/', StringSplitOptions.RemoveEmptyEntries);
            var newSegments = pageURI.Split('/', StringSplitOptions.RemoveEmptyEntries);

            for (var i = currentSegments.Length; i < newSegments.Length; i++) {
                currentSegments = currentSegments.Append("").ToArray();
            }

            pageURI = "";
            for (var i = I18N.USE_PREFIX ? 1 : 0; i < newSegments.Length; i++) {
                var segment = newSegments[i];
                if (segment.StartsWith(URL.EncodeValue("{")) && segment.EndsWith(URL.EncodeValue("}"))) {
                    segment = currentSegments[i];
                }
                if (segment == "") break;
                pageURI = $"{pageURI}/{segment}";
            }
            if (pageURI == "") pageURI = "/";

            try { pageURI = page.CustomURL($"{value}", pageURI); }
            catch (Exception e) {
                Notification.Error(e.Message);
                await PageLoader.Hide(PAGE_LOADER_TASK.CULTURE_CHANGE);
                return;
            } 
            path = I18N.USE_PREFIX ? $"/{CultureInfo.CurrentCulture}{pageURI}" : pageURI;
        }

        // Change prefix or host:
        if (I18N.USE_PREFIX) {
            if (path.StartsWith($"/{CultureInfo.CurrentCulture}/")) {
                path = $"/{value}{path.Substring(path.IndexOf("/", 1))}";
            }
        } else {
            path = $"{URL.Schema()}://{I18N.GetHost(value)}{path}";
        }
        
        // Add query params to path:
        path = URL.WithQuery(path, URL.Query());

        // Set query parameters:
        QueryParams q = new QueryParams();
        q.Set("culture", value);
        q.Set("redirectUri", path);

        // Request redirect:
        await Navigator.NavigateTo(URL.SetQueryParams(API.BASE.SET_CULTURE(), q), forceLoad: true);
    }
    protected override void OnAfterRender(bool firstRender) {
        if (AppEnvironment.IsServer() || !firstRender) return;
        JS.InvokeVoid(JSTempTitle.Remove);
    }
}
