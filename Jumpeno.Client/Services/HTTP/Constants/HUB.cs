namespace Jumpeno.Client.Constants;

public static class HUB {
    public static class Base {
        // URL ----------------------------------------------------------------------------------------------------------------------------
        public static string Prefix => AppSettings.Hub.Base.Prefix;
        public static string URL => $"{AppSettings.Hub.Base.URL}{Prefix}";

        // Endpoints ----------------------------------------------------------------------------------------------------------------------
        // Game:
        public static string Game => $"{URL}/Game";
        // Chat:
        public static string Chat => $"{Prefix}/chat";
    }
}
