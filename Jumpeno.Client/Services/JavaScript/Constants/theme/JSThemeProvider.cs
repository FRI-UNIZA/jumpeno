namespace Jumpeno.Client.Constants;

public static class JSThemeProvider {
    public static readonly string ClassName = typeof(JSThemeProvider).Name;

    public static readonly string Init = $"{ClassName}.Init";
    public static readonly string DarkThemePreferred = $"{ClassName}.DarkThemePreferred";
    public static readonly string SetCustomTheme = $"{ClassName}.SetCustomTheme";
    public static readonly string StartSettingTheme = $"{ClassName}.StartSettingTheme";
    public static readonly string FinishSettingTheme = $"{ClassName}.FinishSettingTheme";
}
