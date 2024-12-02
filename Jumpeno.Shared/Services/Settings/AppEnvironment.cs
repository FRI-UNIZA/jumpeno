namespace Jumpeno.Shared.Services;

#pragma warning disable CS8618

public static class AppEnvironment {
    // Initializers -----------------------------------------------------------------------------------------------------------------------
    public static void Init(
        Func<bool> isServer,
        Func<bool> isController,
        Func<bool> isDevelopment,
        Func<Type, object> getService
    ) {
        IsServerCheck = isServer;
        IsControllerCheck = isController;
        IsDevelopmentCheck = isDevelopment;
        GetServiceOfType = getService;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static Func<bool> IsServerCheck; public static bool IsServer => IsServerCheck();
    private static Func<bool> IsControllerCheck; public static bool IsController => IsControllerCheck();
    private static Func<bool> IsDevelopmentCheck; public static bool IsDevelopment => IsDevelopmentCheck();

    private static Func<Type, object> GetServiceOfType { get; set; }
    public static T GetService<T>() {
        return (T) GetServiceOfType(typeof(T));
    }

    public static string Import(string url) {
        return $"{url}?v={AppSettings.Version}";
    }
}
