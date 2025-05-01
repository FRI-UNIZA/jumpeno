namespace Jumpeno.Shared.Constants;

public static class HUB {
    public static class BASE {
        // URL ----------------------------------------------------------------------------------------------------------------------------
        public static string PREFIX => AppSettings.Hub.Base.Prefix;
        public static string URL => $"{AppSettings.Hub.Base.URL}{PREFIX}";

        // Endpoints ----------------------------------------------------------------------------------------------------------------------
        // Game:
        public static string GAME => $"{URL}/Game";
    }
}
