namespace Jumpeno.Client.Themes;

#pragma warning disable CS8618

public partial class ThemeProvider {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Class:
    public static string ClassNoTheme => JSThemeProvider.ClassNoTheme;
    public static string ClassDarkTheme => JSThemeProvider.ClassDarkTheme;
    public static string ClassLightTheme => JSThemeProvider.ClassLightTheme;

    public static string ClassSettingTheme => JSThemeProvider.ClassSettingTheme;
    public static string ClassSettingThemeAnimation => JSThemeProvider.ClassSettingThemeAnimation;

    public static string ClassThemeTransitionContainer => JSThemeProvider.ClassThemeTransitionContainer;

    public static string Suffix => JSThemeProvider.Suffix;
    public static string ThemeSuffix => JSThemeProvider.ThemeSuffix;
    // Autodetect:
    public static bool ThemeAutodetect { get; private set; }
    // Cascade:
    public const string CascadeAppTheme = $"{nameof(ThemeProvider)}.{nameof(CascadeAppTheme)}";
    public const string CascadeChangeAppTheme = $"{nameof(ThemeProvider)}.{nameof(CascadeChangeAppTheme)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static ThemeProvider? Instance {
        get { return RequestStorage.Get<ThemeProvider>(RequestStorages.ThemeProvider); }
        set { if (value == null) RequestStorage.Delete(RequestStorages.ThemeProvider); else RequestStorage.Set(RequestStorages.ThemeProvider, value); }
    }
    private BaseTheme AppTheme = ThemeType.Default;
    public static string ThemeCSSClass(string classname) {
        return $"{HttpUtility.HtmlEncode(classname).Replace("Theme", "").ToLower()}-theme";
    }
    public static string ThemeCSSClass(BaseTheme theme) {
        return ThemeCSSClass(theme.GetType().Name);
    }
    public static string ThemeCSSClass() {
        var instance = Instance;
        if (instance == null) return ThemeCSSClass(ThemeType.Default);
        return ThemeCSSClass(instance.AppTheme);
    }
    public static string ServerBodyClass() {
        AppEnvironment.AssertServer();
        var c = new CssClass(Window.ClassBody)
        .SetSurface(Surface.Priamary);
        var cookie = GetThemeCookie();
        if (cookie is null) {
            c.Set(ThemeCSSClass(ThemeType.Default));
            c.Set(ClassNoTheme);
        } else {
            c.Set(ThemeCSSClass(cookie));
        }
        return c;
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    // AppSettings:
    public static void Init() {
        InitOnce.Check(nameof(ThemeProvider));
        ThemeAutodetect = AppSettings.Theme.AutoDetect;
    }
    // Component:
    private readonly TaskCompletionSource InitTCS = new();
    public Task Initialization => InitTCS.Task;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ThemeProvider() => Instance = this;

    protected override async Task OnInitializedAsync() {
        await HTTP.Sync(() => {
            var cookie = GetThemeCookie();
            if (ThemeAutodetect && cookie is null) {
                if (AppEnvironment.IsServer) {
                    AppTheme = ThemeType.Default;
                } else {
                    AppTheme = JS.Invoke<bool>(JSThemeProvider.DarkThemePreferred) ? ThemeType.Dark : ThemeType.Light;
                }
            } else if (cookie is null) {
                AppTheme = ThemeType.Default;
            } else {
                AppTheme = CreateThemeByName(cookie);
                if (!AppEnvironment.IsServer) {
                    SetThemeCookie(AppTheme);
                }
            }
            ScrollArea.SetTheme(AppTheme.BodyScrollTheme);
            InitTCS.SetResult();
        });
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    // Get cookie:
    private static string? GetThemeCookie() => AppEnvironment.GetService<CookieStorage>().Get(Cookies.Preference.AppTheme);
    // Set cookie:
    private static void SetThemeCookie(string className) {
        AppEnvironment.GetService<CookieStorage>().Set(new Models.Cookie(
            Cookies.Preference.AppTheme,
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
            return ThemeType.Default;
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> ChangeAppTheme(BaseTheme theme) {
        try {
            await PageLoader.Show(PageLoaderTask.ThemeChange);
            if (AppEnvironment.IsServer) throw new InvalidOperationException("Theme change not allowed on the server!");
            if (theme.GetType().Name == AppTheme.GetType().Name) throw new InvalidOperationException("Theme already set!");
            await HTTP.Sync(() => SetThemeCookie(theme));
            AppTheme = theme;
            return true;
        } catch {
            Notification.Error(Messages.Default.T);
            return false;
        } finally {
            ScrollArea.SavePositions();
            ActionHandler.PreventScroll();
            AnimationHandler.DisableAnimation();
            JS.InvokeVoid(JSThemeProvider.StartSettingTheme);
            JS.InvokeVoid(JSThemeProvider.SetCustomTheme, ThemeCSSClass(AppTheme));
            ScrollArea.SetTheme(AppTheme.BodyScrollTheme);
            StateHasChanged();
            await Task.Yield();
            AnimationHandler.RestoreAnimation();
            await Task.Yield();
            ScrollArea.RestorePositions();
            JS.InvokeVoid(JSThemeProvider.ApplyThemeAnimation);
            await Task.Delay(AppTheme.TransitionExtraSlow);
            ActionHandler.RestoreScroll();
            JS.InvokeVoid(JSThemeProvider.FinishSettingTheme);
            await PageLoader.Hide(PageLoaderTask.ThemeChange);
        }
    }
}
