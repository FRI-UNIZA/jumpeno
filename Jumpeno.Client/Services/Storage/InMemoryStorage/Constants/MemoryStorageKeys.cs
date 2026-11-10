namespace Jumpeno.Client.Constants;

public static class MemoryStorageKeys
{
    // Cookies:
    public const string CookieModal = $"{nameof(Components.CookieModal)}";
    // Disposer:
    public const string Disposer = $"{nameof(Services.Disposer)}";
    // Components:
    public const string Layout = $"{nameof(Base.Layout)}";
    public const string Page = $"{nameof(Base.Page)}";
    // ScrollArea:
    public const string ScrollareaAreas = $"{nameof(ScrollArea)}.{nameof(ScrollareaAreas)}";
    public const string ScrollareaRegisterListeners = $"{nameof(ScrollArea)}.{nameof(ScrollareaRegisterListeners)}";
    // Services:
    public static string ServiceComponent<T>() => $"{nameof(Base.ServiceComponent<T>)}<{typeof(T).Name}>";
    public static string Navigator = $"{nameof(Services.Navigator)}";
    // Theme:
    public const string Theme = $"{nameof(Theme)}";
    // Tokens:
    public static string TokenAccess => TokenType.Access.String();
    public static string TokenRefresh => TokenType.Refresh.String();
    public static string TokenActivation => TokenType.Activation.String();
    public static string TokenPasswordReset => TokenType.PasswordReset.String();
}

