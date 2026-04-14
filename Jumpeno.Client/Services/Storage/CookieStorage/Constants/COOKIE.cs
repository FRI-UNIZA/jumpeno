namespace Jumpeno.Client.Enums;

#pragma warning disable IDE0001
#pragma warning disable IDE0002

public static class Cookies {
    // Cookies ----------------------------------------------------------------------------------------------------------------------------
    public enum Mandatory {
        [StringValue("App.CookiesAccepted")] AppCookiesAccepted,
        [StringValue("AspNetCore.Antiforgery")] AspNetCoreAntiforgery,
        [StringValue("App.RefreshToken")] AppRefershToken
    }

    public enum Preference {
        [StringValue("App.Culture")] AppCulture,
        [StringValue("App.Theme")] AppTheme
    }

    public enum Security {
        [StringValue("App.Recaptcha")] AppRecaptcha
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
            Cookies.Mandatory.AppRefershToken, [
                (DEFAULT_DOMAIN, API.BASE.AUTH_REFRESH),
                (DEFAULT_DOMAIN, API.BASE.AUTH_INVALIDATE),
                (DEFAULT_DOMAIN, API.BASE.AUTH_DELETE)
            ]
        }
    };
}
