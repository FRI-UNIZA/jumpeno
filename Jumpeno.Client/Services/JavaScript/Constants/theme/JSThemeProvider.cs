namespace Jumpeno.Client.Constants;

public static class JSThemeProvider {
    public static readonly string ClassName = nameof(JSThemeProvider);

    public static readonly string Init = $"{ClassName}.{nameof(Init)}";
    public static readonly string DarkThemePreferred = $"{ClassName}.{nameof(DarkThemePreferred)}";
    public static readonly string SetCustomTheme = $"{ClassName}.{nameof(SetCustomTheme)}";
    public static readonly string StartSettingTheme = $"{ClassName}.{nameof(StartSettingTheme)}";
    public static readonly string FinishSettingTheme = $"{ClassName}.{nameof(FinishSettingTheme)}";
}
