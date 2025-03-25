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
    public static string[] Admins { get; private set; }
    public static string JWT_SECRET { get; private set; }
    public static ServerSettingsDatabase Database { get; private set; } public class ServerSettingsDatabase {
        public string DB_HOST { get; init; }
        public int DB_PORT { get; init; }
        public string Database { get; init; }
        public string User { get; init; }
        public string DB_PASSWORD { get; init; }
    }
    public static ServerSettingsEmail Email { get; private set; } public class ServerSettingsEmail {
        public string EMAIL_HOST { get; init; }
        public int EMAIL_PORT { get; init; }
        public string EMAIL_FROM { get; init; }
        public string EMAIL_PASSWORD { get; init; }
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
        Admins = config.GetSection("Admins").Get<string[]>()!;
        JWT_SECRET = config.GetValue<string>("JWT_SECRET")!;
        Database = new() {
            DB_HOST = config.GetValue<string>("Database:DB_HOST")!,
            DB_PORT = config.GetValue<int>("Database:DB_PORT")!,
            Database = config.GetValue<string>("Database:Database")!,
            User = config.GetValue<string>("Database:User")!,
            DB_PASSWORD = config.GetValue<string>("Database:DB_PASSWORD")!
        };
        Email = new() {
            EMAIL_HOST = config.GetValue<string>("Email:EMAIL_HOST")!,
            EMAIL_PORT = config.GetValue<int>("Email:EMAIL_PORT")!,
            EMAIL_FROM = config.GetValue<string>("Email:EMAIL_FROM")!,
            EMAIL_PASSWORD = config.GetValue<string>("Email:EMAIL_PASSWORD")!
        };
    }
}
