namespace Jumpeno.Client.Constants;

public static class JSThemeProvider {
    public static readonly string ClassName = nameof(JSThemeProvider);

    public const string CLASS_NO_THEME = "no-theme";
    public const string CLASS_DARK_THEME = "dark-theme";
    public const string CLASS_LIGHT_THEME = "light-theme";

    public const string CLASS_SETTING_THEME = "setting-theme";
    public const string CLASS_SETTING_THEME_ANIMATION = "setting-theme-animation";

    public const string CLASS_THEME_TRANSITION_CONTAINER = "theme-transition-container";

    public const string SUFFIX = "theme";
    public static string THEME_SUFFIX => $"-{SUFFIX}";

    public static readonly string Init = $"{ClassName}.{nameof(Init)}";
    public static readonly string DarkThemePreferred = $"{ClassName}.{nameof(DarkThemePreferred)}";
    public static readonly string SetCustomTheme = $"{ClassName}.{nameof(SetCustomTheme)}";
    public static readonly string StartSettingTheme = $"{ClassName}.{nameof(StartSettingTheme)}";
    public static readonly string ApplyThemeAnimation = $"{ClassName}.{nameof(ApplyThemeAnimation)}";
    public static readonly string FinishSettingTheme = $"{ClassName}.{nameof(FinishSettingTheme)}";
}
