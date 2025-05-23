namespace Jumpeno.Client.Themes;

#pragma warning disable CS8618

public partial class ThemeProvider {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string NAME = nameof(ThemeProvider);
    public const string TEMPLATE_NAMESPACE = "Constants";
    public const string CLASSNAME_NO_THEME = "no-theme";
    public const string CLASSNAME_THEME_TRANSITION_CONTAINER = "theme-transition-container";
    public static bool THEME_AUTODETECT { get; private set; }

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static ThemeProvider? Instance {
        get { return RequestStorage.Get<ThemeProvider>(NAME); }
        set { if (value == null) RequestStorage.Delete(NAME); else RequestStorage.Set(NAME, value); }
    }
    private BaseTheme Theme = THEME.DEFAULT;
    public static string ThemeCSSClass(string classname) {
        return $"{HttpUtility.HtmlEncode(classname).Replace("Theme", "").ToLower()}-theme";
    }
    public static string ThemeCSSClass(BaseTheme theme) {
        return ThemeCSSClass(theme.GetType().Name);
    }
    public static string ThemeCSSClass() {
        var instance = Instance;
        if (instance == null) return ThemeCSSClass(THEME.DEFAULT);
        return ThemeCSSClass(instance.Theme);
    }
    public static string ServerBodyClass() {
        var c = new CSSClass();
        var cookie = GetThemeCookie();
        if (cookie is null) {
            c.Set(ThemeCSSClass(THEME.DEFAULT));
            c.Set(CLASSNAME_NO_THEME);
        } else {
            c.Set(ThemeCSSClass(cookie));
        }
        return c;
    }
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ThemeProvider() => Instance = this;

    protected override void OnInitialized() {
        var cookie = GetThemeCookie();
        if (THEME_AUTODETECT && cookie is null) {
            if (AppEnvironment.IsServer) {
                Theme = THEME.DEFAULT;
            } else {
                Theme = JS.Invoke<bool>(JSThemeProvider.DarkThemePreferred) ? THEME.DARK : THEME.LIGHT;
            }
        } else if (cookie is null) {
            Theme = THEME.DEFAULT;
        } else {
            Theme = CreateThemeByName(cookie);
            if (!AppEnvironment.IsServer) {
                SetThemeCookie(Theme);
            }
        }        
        ScrollArea.SetTheme(Theme.SCROLL_THEME);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) return;
        await Task.Delay(Theme.TRANSITION_FAST);
        await PageLoader.Hide(PAGE_LOADER_TASK.THEME_CHANGE);
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init() => THEME_AUTODETECT = AppSettings.Theme.AutoDetect;

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static string? GetThemeCookie() => CookieStorage.Get(COOKIE_FUNCTIONAL.APP_THEME);

    private static void SetThemeCookie(string className) {
        CookieStorage.Set(new Cookie(
            COOKIE_FUNCTIONAL.APP_THEME,
            className,
            DateTimeOffset.UtcNow.AddYears(1)
        ));
    }

    private static void SetThemeCookie(BaseTheme theme) => SetThemeCookie(theme.GetType().Name);

    public static BaseTheme CreateThemeByName(string className) {
        try {
            string ns = typeof(ThemeProvider).Namespace!;
            ns = ns.Substring(0, ns.LastIndexOf('.'));
            string fullyQualifiedName = $"{ns}.{TEMPLATE_NAMESPACE}.{className}";
            var type = Type.GetType(fullyQualifiedName)!;
            return (BaseTheme) Activator.CreateInstance(type)!;
        } catch {
            return THEME.DEFAULT;
        }
    }

    // Render -----------------------------------------------------------------------------------------------------------------------------
    public static string RenderThemeVariables(BaseTheme theme) {
        var output = "";
        foreach (var property in Reflex.GetMembers(theme)) {
            output = $"{output}--{@property.Name.ToLower().Replace("_", "-")}: {@property.Value};\n";
        }
        return output;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task ChangeTheme(BaseTheme theme) {
        try {
            if (AppEnvironment.IsServer) throw new InvalidOperationException("Changing theme not allowed on the server!");
            if (theme.GetType().Name == Theme.GetType().Name) return;
            await PageLoader.Show(PAGE_LOADER_TASK.THEME_CHANGE);
            await Task.Delay(Theme.TRANSITION_FAST);
            SetThemeCookie(theme);
            Theme = theme;
            JS.InvokeVoid(JSThemeProvider.StartSettingTheme);
            JS.InvokeVoid(JSThemeProvider.SetCustomTheme, ThemeCSSClass(Theme));
            ScrollArea.SetTheme(Theme.SCROLL_THEME);
            StateHasChanged();
            await Task.Delay(1);
            JS.InvokeVoid(JSThemeProvider.FinishSettingTheme);
        } catch {
            JS.InvokeVoid(JSThemeProvider.FinishSettingTheme);
            await PageLoader.Hide(PAGE_LOADER_TASK.THEME_CHANGE);
        }
    }
}
