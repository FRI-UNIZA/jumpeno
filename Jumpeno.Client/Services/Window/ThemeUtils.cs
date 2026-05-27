namespace Jumpeno.Client.Utils;

public static class ThemeUtils
{
    // Attributes -------------------------------------------------------------------------------------------------------------------------

    public static BaseTheme Theme 
    {
        get 
        {
            return AppEnvironment.MemoryStorage.Get<BaseTheme>(Constants.MemoryStorageKeys.Theme) ?? ThemeType.Default;
        }
        set 
        {
            if (value == null) AppEnvironment.MemoryStorage.Delete(Constants.MemoryStorageKeys.Theme);
            else AppEnvironment.MemoryStorage.Set(Constants.MemoryStorageKeys.Theme, value);
        }
    }

    // Class:
    public static string ClassNoTheme => JSThemeProvider.ClassNoTheme;
    public static string ClassDarkTheme => JSThemeProvider.ClassDarkTheme;
    public static string ClassLightTheme => JSThemeProvider.ClassLightTheme;

    public static string ClassSettingTheme => JSThemeProvider.ClassSettingTheme;
    public static string ClassSettingThemeAnimation => JSThemeProvider.ClassSettingThemeAnimation;

    public static string ClassThemeTransitionContainer => JSThemeProvider.ClassThemeTransitionContainer;

    public static string Suffix => JSThemeProvider.Suffix;
    public static string ThemeSuffix => JSThemeProvider.ThemeSuffix;
    // Utils ------------------------------------------------------------------------------------------------------------------------------
    // Get cookie:
    public static string? GetThemeCookie() => AppEnvironment.GetService<CookieStorage>().Get(Cookies.Preference.AppTheme);
    // Set cookie:
    public static void SetThemeCookie(string className)
    {
        AppEnvironment.GetService<CookieStorage>().Set(new Cookie(
            Cookies.Preference.AppTheme,
            className,
            DateTimeOffset.UtcNow.AddYears(1)
        ));
    }
    public static void SetThemeCookie(BaseTheme theme) => SetThemeCookie(theme.GetType().Name);
    // Theme by name:
    public static BaseTheme CreateThemeByName(string className)
    {
        try
        {
            var type = Type.GetType($"{typeof(BaseTheme).Namespace}.{className}")!;
            return (BaseTheme)Activator.CreateInstance(type)!;
        }
        catch
        {
            return ThemeType.Default;
        }
    }

    public static string ServerBodyClass()
    {
        AppEnvironment.AssertServer();
        var c = new CssClass(Window.ClassBody)
        .SetSurface(Surface.Priamary);
        var cookie = GetThemeCookie();
        if (cookie is null)
        {
            c.Set(ThemeCssClass(ThemeType.Default));
            c.Set(ClassNoTheme);
        }
        else
        {
            c.Set(ThemeCssClass(cookie));
        }
        return c;
    }
    public static string ThemeCssClass(string classname) => $"{HttpUtility.HtmlEncode(classname).Replace("Theme", "").ToLower()}-theme";
    public static string ThemeCssClass(BaseTheme theme) => ThemeCssClass(theme.GetType().Name);
    public static string ThemeCssClass() => ThemeCssClass(Theme);
}
