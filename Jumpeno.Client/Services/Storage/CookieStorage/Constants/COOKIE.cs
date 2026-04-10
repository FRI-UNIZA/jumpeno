namespace Jumpeno.Client.Constants;

#pragma warning disable IDE0001
#pragma warning disable IDE0002

public static class Cookies {
    // Cookies ----------------------------------------------------------------------------------------------------------------------------
    public enum Mandatory {
        [StringValue("App.CookiesAccepted")] APP_COOKIES_ACCEPTED,
        [StringValue("AspNetCore.Antiforgery")] ASP_NET_CORE_ANTIFORGERY,
        [StringValue("App.RefreshToken")] APP_REFRESH_TOKEN
    }

    public enum Preference {
        [StringValue("App.Culture")] APP_CULTURE,
        [StringValue("App.Theme")] APP_THEME
    }

    public enum Security {
        [StringValue("App.Recaptcha")] APP_RECAPTCHA
    }

    // Types ------------------------------------------------------------------------------------------------------------------------------
    public static readonly List<Type> TYPES_REQUIRED = [
        typeof(Cookies.Mandatory)
    ];
    
    public static readonly List<Type> TYPES = [
        typeof(Cookies.Mandatory),
        typeof(Cookies.Preference),
        typeof(Cookies.Security)
    ];

    // Origin (domain and path) -----------------------------------------------------------------------------------------------------------
    public static string DEFAULT_DOMAIN => URL.Domain();
    public static string DEFAULT_PATH => "/";

    // NOTE: Default domain and path values do not have to be specified.
    public static readonly Dictionary<Enum, List<(string DOMAIN, string PATH)>> ORIGIN = new() {
        {
            Cookies.Mandatory.APP_REFRESH_TOKEN, [
                (DEFAULT_DOMAIN, API.BASE.AUTH_REFRESH),
                (DEFAULT_DOMAIN, API.BASE.AUTH_INVALIDATE),
                (DEFAULT_DOMAIN, API.BASE.AUTH_DELETE)
            ]
        }
    };
}
