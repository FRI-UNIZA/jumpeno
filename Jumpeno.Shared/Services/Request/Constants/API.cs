namespace Jumpeno.Shared.Constants;

public static class API {
    public static class BASE {
        // URL ----------------------------------------------------------------------------------------------------------------------------
        public static string URL => $"{AppSettings.Api.Base.URL}{AppSettings.Api.Base.Prefix}";

        // Endpoints ----------------------------------------------------------------------------------------------------------------------
        public static string COOKIE => $"{URL}/Cookie";
        
        public static string CULTURE_SET => $"{URL}/Culture/Set";

        public static string ADMIN_LOGIN => $"{URL}/Admin/Login";
    
        public static string USER_REGISTER => $"{URL}/User/Register";
        public static string USER_ACTIVATE => $"{URL}/User/Activate";
        public static string USER_LOGIN => $"{URL}/User/Login";

        public static string GAME_START => $"{URL}/Game/Start";
        public static string GAME_PAUSE => $"{URL}/Game/Pause";
        public static string GAME_RESUME => $"{URL}/Game/Resume";
        public static string GAME_RESET => $"{URL}/Game/Reset";
    }
}
