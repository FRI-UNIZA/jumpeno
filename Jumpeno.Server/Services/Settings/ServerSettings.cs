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
    public static ServerSettingsJWT JWT { get; private set; } public class ServerSettingsJWT {
        public string AccessSecret { get; init; }
        public string RefreshSecret { get; init; }
    }
    public static ServerSettingsDatabase Database { get; private set; } public class ServerSettingsDatabase {
        public string Version { get; init; }
        public string Host { get; init; }
        public int Port { get; init; }
        public string Database { get; init; }
        public string User { get; init; }
        public string Password { get; init; }
    }
    public static ServerSettingsEmail Email { get; private set; } public class ServerSettingsEmail {
        public string Host { get; init; }
        public int Port { get; init; }
        public string Address { get; init; }
        public string Password { get; init; }
        public bool Mailcatcher { get; init; }
    }
    public static string[] Admins { get; private set; }

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
        JWT = new() {
            AccessSecret = config.GetValue<string>("JWT:AccessSecret")!,
            RefreshSecret = config.GetValue<string>("JWT:RefreshSecret")!
        };
        Database = new() {
            Version = config.GetValue<string>("Database:Version")!,
            Host = Environment.GetEnvironmentVariable("DB_HOST") ?? config.GetValue<string>("Database:Host")!,
            Port = Environment.GetEnvironmentVariable("DB_PORT") is string portDB ? int.Parse(portDB) : config.GetValue<int>("Database:Port")!,
            Database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? config.GetValue<string>("Database:Database")!,
            User = Environment.GetEnvironmentVariable("DB_USER") ?? config.GetValue<string>("Database:User")!,
            Password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? config.GetValue<string>("Database:Password")!
        };
        Email = new() {
            Host = Environment.GetEnvironmentVariable("EMAIL_HOST") ?? config.GetValue<string>("Email:Host")!,
            Port = Environment.GetEnvironmentVariable("EMAIL_PORT") is string portEmail ? int.Parse(portEmail) : config.GetValue<int>("Email:Port")!,
            Address = Environment.GetEnvironmentVariable("EMAIL_ADDRESS") ?? config.GetValue<string>("Email:Address")!,
            Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? config.GetValue<string>("Email:Password")!,
            Mailcatcher = config.GetValue<bool>("Email:Mailcatcher")!
        };
        Admins = config.GetSection("Admins").Get<string[]>()!;
    }
}
