namespace Jumpeno.Client.Themes;

#pragma warning disable CS8618

public partial class ThemeProvider {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Class:
    public const string CLASS_BODY = "body";
    public const string CLASS_NO_THEME = "no-theme";
    public const string CLASS_THEME_TRANSITION_CONTAINER = "theme-transition-container";
    // Autodetect:
    public static bool THEME_AUTODETECT { get; private set; }
    // Cascade:
    public const string CASCADE_APP_THEME = $"{nameof(ThemeProvider)}.{nameof(CASCADE_APP_THEME)}";
    public const string CASCADE_CHANGE_APP_THEME = $"{nameof(ThemeProvider)}.{nameof(CASCADE_CHANGE_APP_THEME)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static ThemeProvider? Instance {
        get { return RequestStorage.Get<ThemeProvider>(REQUEST_STORAGE.THEME_PROVIDER); }
        set { if (value == null) RequestStorage.Delete(REQUEST_STORAGE.THEME_PROVIDER); else RequestStorage.Set(REQUEST_STORAGE.THEME_PROVIDER, value); }
    }
    private BaseTheme AppTheme = THEME.DEFAULT;
    public static string ThemeCSSClass(string classname) {
        return $"{HttpUtility.HtmlEncode(classname).Replace("Theme", "").ToLower()}-theme";
    }
    public static string ThemeCSSClass(BaseTheme theme) {
        return ThemeCSSClass(theme.GetType().Name);
    }
    public static string ThemeCSSClass() {
        var instance = Instance;
        if (instance == null) return ThemeCSSClass(THEME.DEFAULT);
        return ThemeCSSClass(instance.AppTheme);
    }
    public static string ServerBodyClass() {
        AppEnvironment.AssertServer();
        var c = new CSSClass(CLASS_BODY)
        .SetSurface(SURFACE.PRIMARY);
        var cookie = GetThemeCookie();
        if (cookie is null) {
            c.Set(ThemeCSSClass(THEME.DEFAULT));
            c.Set(CLASS_NO_THEME);
        } else {
            c.Set(ThemeCSSClass(cookie));
        }
        return c;
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    // AppSettings:
    public static void Init() {
        InitOnce.Check(nameof(ThemeProvider));
        THEME_AUTODETECT = AppSettings.Theme.AutoDetect;
    }
    // Component:
    private readonly TaskCompletionSource InitTCS = new();
    public Task Initialization => InitTCS.Task;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ThemeProvider() => Instance = this;

    protected override async Task OnInitializedAsync() {
        await HTTP.Sync(() => {
            var cookie = GetThemeCookie();
            if (THEME_AUTODETECT && cookie is null) {
                if (AppEnvironment.IsServer) {
                    AppTheme = THEME.DEFAULT;
                } else {
                    AppTheme = JS.Invoke<bool>(JSThemeProvider.DarkThemePreferred) ? THEME.DARK : THEME.LIGHT;
                }
            } else if (cookie is null) {
                AppTheme = THEME.DEFAULT;
            } else {
                AppTheme = CreateThemeByName(cookie);
                if (!AppEnvironment.IsServer) {
                    SetThemeCookie(AppTheme);
                }
            }
            ScrollArea.SetTheme(AppTheme.BODY_SCROLL_THEME);
            InitTCS.SetResult();
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!await PageLoader.IsActiveTask(PAGE_LOADER_TASK.THEME_CHANGE)) return;
        await Task.Delay(AppTheme.TRANSITION_FAST);
        await PageLoader.Hide(PAGE_LOADER_TASK.THEME_CHANGE);
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    // Get cookie:
    private static string? GetThemeCookie() => CookieStorage.Get(COOKIE.PREFERENCES.APP_THEME);
    // Set cookie:
    private static void SetThemeCookie(string className) {
        CookieStorage.Set(new Cookie(
            COOKIE.PREFERENCES.APP_THEME,
            className,
            DateTimeOffset.UtcNow.AddYears(1)
        ));
    }
    private static void SetThemeCookie(BaseTheme theme) => SetThemeCookie(theme.GetType().Name);
    // Theme by name:
    private static BaseTheme CreateThemeByName(string className) {
        try {
            var type = Type.GetType($"{typeof(BaseTheme).Namespace}.{className}")!;
            return (BaseTheme)Activator.CreateInstance(type)!;
        } catch {
            return THEME.DEFAULT;
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task ChangeAppTheme(BaseTheme theme) {
        try {
            if (AppEnvironment.IsServer) throw new InvalidOperationException("Changing theme not allowed on the server!");
            if (theme.GetType().Name == AppTheme.GetType().Name) return;
            await PageLoader.Show(PAGE_LOADER_TASK.THEME_CHANGE);
            await HTTP.Sync(() => SetThemeCookie(theme));
            AppTheme = theme;
            JS.InvokeVoid(JSThemeProvider.StartSettingTheme);
            JS.InvokeVoid(JSThemeProvider.SetCustomTheme, ThemeCSSClass(AppTheme));
            ScrollArea.SetTheme(AppTheme.BODY_SCROLL_THEME);
            StateHasChanged();
            await Task.Yield();
            JS.InvokeVoid(JSThemeProvider.FinishSettingTheme);
        } catch {
            JS.InvokeVoid(JSThemeProvider.FinishSettingTheme);
            await PageLoader.Hide(PAGE_LOADER_TASK.THEME_CHANGE);
        }
    }
}
