namespace Jumpeno.Client.Constants;

public static class RequestStorages {
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
    // Services:
    public static string SERVICE_COMPONENT<T>() => $"{nameof(ServiceComponent<T>)}<{typeof(T).Name}>";
    public static string STATIC_SERVICE<T>() => $"{nameof(StaticService<T>)}<{typeof(T).Name}>";
    // Theme:
    public const string THEME_PROVIDER = $"{nameof(ThemeProvider)}";
    // Tokens:
    public static string TOKEN_ACCESS => TokenType.Access.String();
    public static string TOKEN_REFRESH => TokenType.Refresh.String();
    public static string TOKEN_ACTIVATION => TokenType.Activation.String();
    public static string TOKEN_PASSWORD_RESET => TokenType.PasswordReset.String();
}
