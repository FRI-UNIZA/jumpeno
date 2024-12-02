namespace Jumpeno.Shared.Constants;

public static class API {
    public static class BASE {
        // URL ----------------------------------------------------------------------------------------------------------------------------
        public static string URL => $"{AppSettings.Api.Base.URL}{AppSettings.Api.Base.Prefix}";

        // Endpoints ----------------------------------------------------------------------------------------------------------------------
        public static string COOKIE => $"{URL}/Cookie";
        public static string CULTURE_SET => $"{URL}/Culture/Set";
        public static string GAME_START => $"{URL}/Game/Start";
        public static string GAME_RESET => $"{URL}/Game/Reset";
    }
}
