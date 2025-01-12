namespace Jumpeno.Shared.Utils;

public static class Format {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static JsonSerializerOptions JSON_OPTIONS_PRETTY = new() { WriteIndented = true };

    // Time -------------------------------------------------------------------------------------------------------------------------------
    public static string Ms_To_MMSS(double ms, bool space = false) {
        int seconds = (int) Math.Floor(ms / 1000);
        int displayMinutes = seconds / 60;
        int displaySeconds = seconds % 60;
        string s = space ? " " : "";
        return $"{displayMinutes:D2}{s}:{s}{displaySeconds:D2}";
    }

    // Numbers ----------------------------------------------------------------------------------------------------------------------------
    public static string Double(double? value, int decimals = 2) => value == null ? "-" : value.Value.ToString($"F{Math.Max(decimals, 0)}");
    public static string DoubleWhole(double? value) => Double(value, 0);

    // Objects ----------------------------------------------------------------------------------------------------------------------------
    public static string JSON(object item) => JsonSerializer.Serialize(item);
    public static string JSON_PRETTY(object item) => JsonSerializer.Serialize(item, JSON_OPTIONS_PRETTY);
}
