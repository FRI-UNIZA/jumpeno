namespace Jumpeno.Server.Services;

using Newtonsoft.Json;

#pragma warning disable CS8618

public static class ServerSettings {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static string AllowedHosts { get; private set; }
    public static LoggingSettings Logging { get; private set; }

    // Initializer ------------------------------------------------------------------------------------------------------------------------
    public static void Init(IConfiguration config) {
        AllowedHosts = config.GetValue<string>("AllowedHosts")!;
        Logging = new LoggingSettings(
            new LogLevelSettings(
                config.GetValue<string>("Logging:LogLevel:Default")!,
                config.GetValue<string>("Logging:LogLevel:Microsoft.AspNetCore")!
            )
        );
    }
}

// LoggingSettings ------------------------------------------------------------------------------------------------------------------------
public record LoggingSettings (
    LogLevelSettings LogLevel
) {}

public record LogLevelSettings (
    string Default,
    [JsonProperty("Microsoft.AspNetCore")]
    string Microsoft_AspNetCore
) {}
// END LoggingSettings --------------------------------------------------------------------------------------------------------------------
