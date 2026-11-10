namespace Jumpeno.Client.Constants;

public static class API {
    public static class Base {
        // URL ----------------------------------------------------------------------------------------------------------------------------
        public static string Prefix => AppSettings.Api.Base.Prefix;
        public static string Url => $"{AppSettings.Api.Base.URL}{Prefix}";

        // Endpoints ----------------------------------------------------------------------------------------------------------------------
        // Cookie:
        public static string CookieSet => $"{Url}/Cookie/Set";
        // Culture:
        public static string CultureRedirect => $"{Url}/Culture/Redirect";
        // Admin:
        public static string AdminLogin => $"{Url}/Admin/Login";
        public static string AdminDbCredentials => $"{Url}/Admin/DBCredentials";
        public static string AdminEmailPassword => $"{Url}/Admin/EmailPassword";
        public static string AdminEmailBackupKeys => $"{Url}/Admin/EmailBackupKeys";
        // Auth:
        public static string AuthRefresh => $"{Url}/Auth/Refresh";
        public static string AuthInvalidate => $"{Url}/Auth/Invalidate";
        public static string AuthDelete => $"{Url}/Auth/Delete";
        // User:
        public static string UserRegister => $"{Url}/User/Register";
        public static string UserSendActivation => $"{Url}/User/SendActivation";
        public static string UserActivate => $"{Url}/User/Activate";
        public static string UserLogin => $"{Url}/User/Login";
        public static string UserPasswordResetRequest => $"{Url}/User/PasswordResetRequest";
        public static string UserPasswordReset => $"{Url}/User/PasswordReset";
        public static string UserPasswordChange => $"{Url}/User/PasswordChange";
        public static string UserProfile => $"{Url}/User/Profile";
        public static string UserUpdate => $"{Url}/User/Update";
        public static string UserDelete => $"{Url}/User/Delete";
        // Game:
        public static string GameMaps => $"{Url}/Game/Maps";
        public static string GameMap => $"{Url}/Game/Map";
        public static string GameStart => $"{Url}/Game/Start";
        public static string GamePause => $"{Url}/Game/Pause";
        public static string GameToggle => $"{Url}/Game/Toggle";
        public static string GameDelete => $"{Url}/Game/Delete";
        public static string GameSetPlayerReady => $"{Url}/Game/SetPlayerReady";
        public static string GameKickPlayer => $"{Url}/Game/KickPlayer";
    }

    public static class Google  {
        // URL ----------------------------------------------------------------------------------------------------------------------------
        public static string Url => AppSettings.Api.Google.URL;

        // ReCAPTCHA ----------------------------------------------------------------------------------------------------------------------
        public static class Recaptcha  {
            public static string Prefix => AppSettings.Api.Google.ReCAPTCHA.Prefix;
            public static string Url => $"{Google.Url}{Prefix}";

            // Endpoints ------------------------------------------------------------------------------------------------------------------
            public static string API => $"{Url}.js";
            public static string SiteVerify => $"{Url}/siteverify";
        }
    }
}
