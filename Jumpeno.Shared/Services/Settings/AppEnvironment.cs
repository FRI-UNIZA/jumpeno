namespace Jumpeno.Shared.Services;

#pragma warning disable CS8618

public static class AppEnvironment {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
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

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public static bool IsServer => IsServerCheck(); private static Func<bool> IsServerCheck;
    public static bool IsClient => !IsServer;
    public static bool IsController => IsControllerCheck(); private static Func<bool> IsControllerCheck;
    public static bool IsDevelopment => IsDevelopmentCheck(); private static Func<bool> IsDevelopmentCheck;
    public static bool IsProduction => !IsDevelopment;

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateServer() => Checker.Validate(!IsServer, new Error("Not a server environment!"));
    public static void CheckServer() => Checker.Check(ValidateServer());

    public static List<Error> ValidateClient() => Checker.Validate(IsServer, new Error("Not a client environment!"));
    public static void CheckClient() => Checker.Check(ValidateClient());
    
    public static List<Error> ValidateController() => Checker.Validate(!IsController, new Error("Not a controller!"));
    public static void CheckController() => Checker.Check(ValidateController());

    public static List<Error> ValidateDevelopment() => Checker.Validate(!IsDevelopment, new Error("Not a development environment!"));
    public static void CheckDevelopment() => Checker.Check(ValidateDevelopment());

    public static List<Error> ValidateProduction() => Checker.Validate(IsDevelopment, new Error("Not a production environment!"));
    public static void CheckProduction() => Checker.Check(ValidateProduction());

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private static Func<Type, object> GetServiceOfType { get; set; }
    public static T GetService<T>() => (T) GetServiceOfType(typeof(T));
    public static string Import(string url) => $"{url}?v={AppSettings.Version}";
}
