namespace Jumpeno.Shared.Constants;

public static class API {
    public static class BASE {
        // URL ----------------------------------------------------------------------------------------------------------------------------
        public static string URL => $"{AppSettings.Api.Base.URL}{AppSettings.Api.Base.Prefix}";

        // Endpoints ----------------------------------------------------------------------------------------------------------------------
        public static string COOKIE => $"{URL}/Cookie";
        
        public static string CULTURE_SET => $"{URL}/Culture/Set";

        public static string GAME_START => $"{URL}/Game/Start";
        public static string GAME_PAUSE => $"{URL}/Game/Pause";
        public static string GAME_RESUME => $"{URL}/Game/Resume";
        public static string GAME_RESET => $"{URL}/Game/Reset";

        public static string USER_LOGIN_ADMIN => $"{URL}/User/LogInAdmin";
        public static string USER_PROFILE_ADMIN => $"{URL}/User/ProfileAdmin";
        public static string USER_CREATE => $"{URL}/User/Create";
        public static string USER_READ => $"{URL}/User/Read";
        public static string USER_UPDATE => $"{URL}/User/Update";
        public static string USER_DELETE => $"{URL}/User/Delete";
        public static string USER_REGISTER => $"{URL}/User/Register";

        public static string DB_DOWNLOAD => $"{URL}/DB/Download";
        public static string DB_ADMIN_TEST => $"{URL}/DB/AdminTest";
    }
}
