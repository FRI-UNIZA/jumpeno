namespace Jumpeno.Client.Components;

public partial class SelectCulture {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "select-culture";
    // Delay:
    public const int CHANGE_DELAY = 300; // ms

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly SelectViewModel<string> VM = new(new(
        Options: [.. I18N.LANGUAGES.Select((x, index) => new SelectOption<string>(index, x, x.ToUpper()))],
        DefaultValue: new(Array.IndexOf(I18N.LANGUAGES, I18N.Culture), I18N.Culture, I18N.Culture.ToUpper()),
        OnSelect: new(OnSelect)
    ));

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentAfterRender(bool firstRender) {
        if (AppEnvironment.IsServer || !firstRender) return;
        JS.InvokeVoid(JSTempTitle.Remove);
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private static async Task OnSelect(SelectEvent<string> ev) {
        await PageLoader.Show(PAGE_LOADER_TASK.CULTURE_CHANGE);
        await Task.Delay(CHANGE_DELAY);
        await ChangeCulture(ev);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task Init() {
        await HTTP.Sync(() => {
            var cookie = CookieStorage.Get(COOKIE.PREFERENCES.APP_CULTURE);
            if (cookie is null) return;
            CookieStorage.Set(new Cookie(
                COOKIE.PREFERENCES.APP_CULTURE,
                cookie,
                DateTimeOffset.UtcNow.AddYears(1)
            ));
        });
    }

    private static async Task ChangeCulture(SelectEvent<string> ev) {
        var value = ev.After.Value!;

        if (I18N.Culture == value) return;

        // Access info:
        var path = URL.Path();

        // Update uri with parameters:
        var page = Page.Current;
        if (page is not Error404Page) {
            if (page is Error401Page || page is Error403Page) {
                try {
                    var type = Page.Type(Layout.Current.Body) ?? throw new InvalidOperationException();
                    page = ((Page?) Activator.CreateInstance(type)) ?? throw new InvalidCastException();
                } catch {
                    Navigator.Refresh();
                    return;
                }
            }
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
        QueryParams q = new();
        q.Set("culture", value);
        q.Set("redirectURI", path);

        // Request redirect:
        await Navigator.NavigateTo(URL.SetQueryParams(API.BASE.CULTURE_REDIRECT, q), forceLoad: true);
    }
}
