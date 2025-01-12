namespace Jumpeno.Server.Services;

#pragma warning disable CS8618

public static class ServerSettings {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static int Port { get; private set; }
    public static string AllowedHosts { get; private set; }
    public static ServerSettingsLogging Logging { get; private set; } public class ServerSettingsLogging {
        public ServerSettingsLoggingLogLevel LogLevel { get; init; } public class ServerSettingsLoggingLogLevel {
            public string Default { get; init; }
            public string Microsoft_AspNetCore { get; init; }
        }
    }

    // Initializer ------------------------------------------------------------------------------------------------------------------------
    public static void Init(IConfiguration config) {
        Port = config.GetValue<int>("Port");
        AllowedHosts = config.GetValue<string>("AllowedHosts")!;
        Logging = new() {
            LogLevel = new() {
                Default = config.GetValue<string>("Logging:LogLevel:Default")!,
                Microsoft_AspNetCore = config.GetValue<string>("Logging:LogLevel:Microsoft.AspNetCore")!
            }
        };
    }
}
