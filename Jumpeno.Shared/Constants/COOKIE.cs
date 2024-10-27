namespace Jumpeno.Shared.Constants;

public enum COOKIE_MANDATORY {
    [StringValue("App.CookiesAccepted")]
    APP_COOKIES_ACCEPTED,
    [StringValue("AspNetCore.Antiforgery")]
    ASP_NET_CORE_ANTIFORGERY
}

public enum COOKIE_FUNCTIONAL {
    [StringValue("App.Culture")]
    APP_CULTURE,
    [StringValue("App.Theme")]
    APP_THEME
}

public static class COOKIE {
    public const string NAMESPACE = "Jumpeno.Shared.Constants";

    // NOTE: Use enum ToString() value as key to save cookie domain and path.
    // (Default domain and path values do not have to be specified.)
    public static readonly Dictionary<string, string> DOMAIN = new() {};
    public static readonly Dictionary<string, string> PATH = new() {};

    public static readonly List<Type> TYPES = [
        typeof(COOKIE_MANDATORY),
        typeof(COOKIE_FUNCTIONAL)
    ];
    public static readonly List<Type> TYPES_REQUIRED = [
        typeof(COOKIE_MANDATORY)
    ];
}
