namespace Jumpeno.Client.Services;

#pragma warning disable CS8618

public static class AppEnvironment {
    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public static bool IsServer => IsServerCheck(); private static Func<bool> IsServerCheck;
    public static bool IsClient => !IsServer;
    public static bool IsController => IsAPI || IsHub;
    public static bool IsAPI => IsAPICheck(); private static Func<bool> IsAPICheck;
    public static bool IsHub => IsHubCheck(); private static Func<bool> IsHubCheck;
    public static bool IsDevelopment => IsDevelopmentCheck(); private static Func<bool> IsDevelopmentCheck;
    public static bool IsProduction => !IsDevelopment;

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateServer() => Checker.Validate(!IsServer, ERROR.DEFAULT.SetInfo("Not a server environment!"));
    public static void CheckServer() => Checker.Assert(ValidateServer());

    public static List<Error> ValidateClient() => Checker.Validate(IsServer, ERROR.DEFAULT.SetInfo("Not a client environment!"));
    public static void CheckClient() => Checker.Assert(ValidateClient());
    
    public static List<Error> ValidateController() => Checker.Validate(!IsController, ERROR.DEFAULT.SetInfo("Not a controller!"));
    public static void CheckController() => Checker.Assert(ValidateController());

    public static List<Error> ValidateAPI() => Checker.Validate(!IsAPI, ERROR.DEFAULT.SetInfo("Not an API!"));
    public static void CheckAPI() => Checker.Assert(ValidateAPI());

    public static List<Error> ValidateHub() => Checker.Validate(!IsHub, ERROR.DEFAULT.SetInfo("Not a hub!"));
    public static void CheckHub() => Checker.Assert(ValidateHub());

    public static List<Error> ValidateDevelopment() => Checker.Validate(!IsDevelopment, ERROR.DEFAULT.SetInfo("Not a development environment!"));
    public static void CheckDevelopment() => Checker.Assert(ValidateDevelopment());

    public static List<Error> ValidateProduction() => Checker.Validate(IsDevelopment, ERROR.DEFAULT.SetInfo("Not a production environment!"));
    public static void CheckProduction() => Checker.Assert(ValidateProduction());

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private static Func<Type, object> GetServiceOfType { get; set; }
    public static T GetService<T>() => (T) GetServiceOfType(typeof(T));
    public static string Import(string url) => $"{url}?v={AppSettings.Version}";

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(
        Func<bool> isServer,
        Func<bool> isAPI,
        Func<bool> isHub,
        Func<bool> isDevelopment,
        Func<Type, object> getService
    ) {
        IsServerCheck = isServer;
        IsAPICheck = isAPI;
        IsHubCheck = isHub;
        IsDevelopmentCheck = isDevelopment;
        GetServiceOfType = getService;
    }
}
