namespace Jumpeno.Shared.Constants;

public static class API {
    public static class BASE {
        // URL ----------------------------------------------------------------------------------------------------------------------------
        public static string URL => $"{AppSettings.Api.Base.URL}{AppSettings.Api.Base.Prefix}";

        // Endpoints ----------------------------------------------------------------------------------------------------------------------
        // Cookie:
        public static string COOKIE_SET => $"{URL}/Cookie/Set";
        // Culture:
        public static string CULTURE_REDIRECT => $"{URL}/Culture/Redirect";
        // Admin:
        public static string ADMIN_LOGIN => $"{URL}/Admin/Login";
        // Auth:
        public static string AUTH_REFRESH => $"{URL}/Auth/Refresh";
        public static string AUTH_INVALIDATE => $"{URL}/Auth/Invalidate";
        public static string AUTH_DELETE => $"{URL}/Auth/Delete";
        // User:
        public static string USER_REGISTER => $"{URL}/User/Register";
        public static string USER_ACTIVATE => $"{URL}/User/Activate";
        public static string USER_LOGIN => $"{URL}/User/Login";
        public static string USER_PASSWORD_RESET_REQUEST => $"{URL}/User/PasswordResetRequest";
        public static string USER_PASSWORD_RESET => $"{URL}/User/PasswordReset";
        public static string USER_PROFILE => $"{URL}/User/Profile";
        // Game:
        public static string GAME_START => $"{URL}/Game/Start";
        public static string GAME_PAUSE => $"{URL}/Game/Pause";
        public static string GAME_RESUME => $"{URL}/Game/Resume";
        public static string GAME_RESET => $"{URL}/Game/Reset";
    }
}
