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
    public static ServerSettingsAuth Auth { get; private set; } public class ServerSettingsAuth {
        public string Pepper { get; init; }
        public ServerSettingsAuthJWT JWT { get; init; } public class ServerSettingsAuthJWT {
            public string AccessSecret { get; init; }
            public string RefreshSecret { get; init; }
            public string DataSecret { get; init; }
        }
        public string[] Admins { get; init; }
    }
    public static ServerSettingsDatabase Database { get; private set; } public class ServerSettingsDatabase {
        public string Version { get; init; }
        public string ConnectionString { get; init; }
    }
    public static ServerSettingsEmail Email { get; private set; } public class ServerSettingsEmail {
        public string Host { get; init; }
        public int Port { get; init; }
        public string Password { get; init; }
        public string BackupKeys { get; init; }
        public string AppPassword { get; init; }
        public bool Mailcatcher { get; init; }
    }
    public static ServerSettingsExpiration Expiration { get; private set; } public class ServerSettingsExpiration {
        public ServerSettingsExpirationAccessToken AccessToken { get; init; }
        public class ServerSettingsExpirationAccessToken {
            public int Minutes { get; init; }
        }
        public ServerSettingsExpirationRefreshToken RefreshToken { get; init; }
        public class ServerSettingsExpirationRefreshToken {
            public int Hours { get; init; }
        }
        public ServerSettingsExpirationActivationToken ActivationToken { get; init; }
        public class ServerSettingsExpirationActivationToken {
            public int Hours { get; init; }
        }
        public ServerSettingsExpirationPasswordResetToken PasswordResetToken { get; init; }
        public class ServerSettingsExpirationPasswordResetToken {
            public int Minutes { get; init; }
        }
    }
    public static ServerSettingsSchedule Schedule { get; private set; } public class ServerSettingsSchedule {
        public ServerSettingsScheduleActivationCleaner ActivationCleaner { get; init; }
        public class ServerSettingsScheduleActivationCleaner {
            public int Minutes { get; init; }
        }
        public ServerSettingsScheduleRefreshCleaner RefreshCleaner { get; init; }
        public class ServerSettingsScheduleRefreshCleaner {
            public int Minutes { get; init; }
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
        Auth = new() {
            Pepper = config.GetValue<string>("Auth:Pepper")!,
            JWT = new() {
                AccessSecret = config.GetValue<string>("Auth:JWT:AccessSecret")!,
                RefreshSecret = config.GetValue<string>("Auth:JWT:RefreshSecret")!,
                DataSecret = config.GetValue<string>("Auth:JWT:DataSecret")!
            },
            Admins = config.GetSection("Auth:Admins").Get<string[]>()!
        };
        Database = new() {
            Version = config.GetValue<string>("Database:Version")!,
            ConnectionString = config.GetConnectionString("DefaultConnection") ?? config.GetValue<string>("Database:ConnectionString")!
        };
        Email = new() {
            Host = Environment.GetEnvironmentVariable("EMAIL_HOST") ?? config.GetValue<string>("Email:Host")!,
            Port = Environment.GetEnvironmentVariable("EMAIL_PORT") is string portEmail ? int.Parse(portEmail) : config.GetValue<int>("Email:Port")!,
            Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? config.GetValue<string>("Email:Password")!,
            BackupKeys = Environment.GetEnvironmentVariable("EMAIL_BACKUP_KEYS") ?? config.GetValue<string>("Email:BackupKeys")!,
            AppPassword = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD") ?? config.GetValue<string>("Email:AppPassword")!,
            Mailcatcher = config.GetValue<bool>("Email:Mailcatcher")!
        };
        Expiration = new() {
            AccessToken = new() { Minutes = config.GetValue<int>("Expiration:AccessToken:Minutes")! },
            RefreshToken = new() { Hours = config.GetValue<int>("Expiration:RefreshToken:Hours")! },
            ActivationToken = new() { Hours = config.GetValue<int>("Expiration:ActivationToken:Hours")! },
            PasswordResetToken = new() { Minutes = config.GetValue<int>("Expiration:PasswordResetToken:Minutes")! }
        };
        Schedule = new() {
            ActivationCleaner = new() { Minutes = config.GetValue<int>("Schedule:ActivationCleaner:Minutes")! },
            RefreshCleaner = new() { Minutes = config.GetValue<int>("Schedule:RefreshCleaner:Minutes")! }
        };
    }
}
