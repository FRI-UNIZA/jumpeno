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
        IsServer = isServer;
        IsController = isController;
        IsDevelopment = isDevelopment;
        GetServiceOfType = getService;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static Func<bool> IsServer { get; private set; }
    public static Func<bool> IsController { get; private set; }
    public static Func<bool> IsDevelopment { get; private set; }
    private static Func<Type, object> GetServiceOfType { get; set; }
    public static T GetService<T>() {
        return (T) GetServiceOfType(typeof(T));
    }
    public static string Import(string url) {
        return $"{url}?v={AppSettings.Version}";
    }
}
