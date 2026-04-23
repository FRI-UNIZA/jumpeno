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
    public static readonly List<Type> TypesRequired = [
        typeof(Cookies.Mandatory)
    ];
    
    public static readonly List<Type> Types = [
        typeof(Cookies.Mandatory),
        typeof(Cookies.Preference),
        typeof(Cookies.Security)
    ];

    // Origin (domain and path) -----------------------------------------------------------------------------------------------------------
    public static string DefaultDomain => URL.Domain();
    public static string DefaultPath => "/";

    // NOTE: Default domain and path values do not have to be specified.
    public static readonly Dictionary<Enum, List<(string DOMAIN, string PATH)>> Origin = new() {
        {
            Cookies.Mandatory.AppRefershToken, [
                (DefaultDomain, API.Base.AuthRefresh),
                (DefaultDomain, API.Base.AuthInvalidate),
                (DefaultDomain, API.Base.AuthDelete)
            ]
        }
    };
}
