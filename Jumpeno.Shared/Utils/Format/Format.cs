namespace Jumpeno.Shared.Utils;

public static class Format {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly JsonSerializerOptions JSON_OPTIONS_PRETTY = new() { WriteIndented = true };

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

    // Strings ----------------------------------------------------------------------------------------------------------------------------
    public static string OfPlayers(int count) {
        if (count < 0) throw new InvalidDataException("Incorrect number of players");
        if (count == 0) return I18N.T("players0");
        if (count == 1) return I18N.T("players1");
        if (count <= 4) return I18N.T("players2+");
        return I18N.T("players5+");
    }

    // Objects ----------------------------------------------------------------------------------------------------------------------------
    public static string JSON(object item) => JsonSerializer.Serialize(item);
    public static string JSON_PRETTY(object item) => JsonSerializer.Serialize(item, JSON_OPTIONS_PRETTY);
}
