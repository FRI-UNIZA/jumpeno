namespace Jumpeno.Client.Constants;

public static class JSThemeProvider {
    public static readonly string ClassName = nameof(JSThemeProvider);

    public const string ClassNoTheme = "no-theme";
    public const string ClassDarkTheme = "dark-theme";
    public const string ClassLightTheme = "light-theme";

    public const string ClassSettingTheme = "setting-theme";
    public const string ClassSettingThemeAnimation = "setting-theme-animation";

    public const string ClassThemeTransitionContainer = "theme-transition-container";

    public const string Suffix = "theme";
    public static string ThemeSuffix => $"-{Suffix}";

    public static readonly string Init = $"{ClassName}.{nameof(Init)}";
    public static readonly string DarkThemePreferred = $"{ClassName}.{nameof(DarkThemePreferred)}";
    public static readonly string SetCustomTheme = $"{ClassName}.{nameof(SetCustomTheme)}";
    public static readonly string StartSettingTheme = $"{ClassName}.{nameof(StartSettingTheme)}";
    public static readonly string ApplyThemeAnimation = $"{ClassName}.{nameof(ApplyThemeAnimation)}";
    public static readonly string FinishSettingTheme = $"{ClassName}.{nameof(FinishSettingTheme)}";
}
