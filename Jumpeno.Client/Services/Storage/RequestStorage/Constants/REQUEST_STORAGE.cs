namespace Jumpeno.Client.Constants;

public static class REQUEST_STORAGE {
    // Cookies:
    public const string COOKIE_MODAL = $"{nameof(CookieModal)}";
    // Disposer:
    public const string DISPOSER = $"{nameof(Disposer)}";
    // Components:
    public const string LAYOUT = $"{nameof(Layout)}";
    public const string PAGE = $"{nameof(Page)}";
    // Database:
    public const string DB = $"{nameof(DB)}";
    // URL:
    public const string URL = $"{nameof(URL)}";
    // ScrollArea:
    public const string SCROLLAREA_AREAS = $"{nameof(ScrollArea)}.{nameof(SCROLLAREA_AREAS)}";
    public const string SCROLLAREA_REGISTER_LISTENERS = $"{nameof(ScrollArea)}.{nameof(SCROLLAREA_REGISTER_LISTENERS)}";
    // Notifications:
    public const string SERVER_NOTIFICATIONS = $"{nameof(Notification)}.{nameof(SERVER_NOTIFICATIONS)}";
    // Services:
    public static string SERVICE_COMPONENT<T>() => $"{nameof(ServiceComponent<T>)}<{typeof(T).Name}>";
    public static string STATIC_SERVICE<T>() => $"{nameof(StaticService<T>)}<{typeof(T).Name}>";
    // Theme:
    public const string THEME_PROVIDER = $"{nameof(ThemeProvider)}";
    // Tokens:
    public static string TOKEN_ACCESS => TOKEN_TYPE.ACCESS.String();
    public static string TOKEN_REFRESH => TOKEN_TYPE.REFRESH.String();
    public static string TOKEN_ACTIVATION => TOKEN_TYPE.ACTIVATION.String();
    public static string TOKEN_PASSWORD_RESET => TOKEN_TYPE.PASSWORD_RESET.String();
}
