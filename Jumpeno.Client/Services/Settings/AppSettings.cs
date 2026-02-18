namespace Jumpeno.Client.Services;

#pragma warning disable CS8618

public static class AppSettings {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static string Name { get; private set; }
    public static string Email { get; private set; }
    public static string Version { get; private set; }
    public static bool Prerender { get; private set; }
    public static bool Redirect { get; private set; }
    public static bool Bundle { get; private set; }
    public static AppSettingsApi Api { get; private set; } public class AppSettingsApi {
        public AppSettingsApiBase Base { get; init; } public class AppSettingsApiBase {
            public string URL { get; init; }
            public string Prefix { get; init; }
        }
        public AppSettingsApiGoogle Google { get; init; } public class AppSettingsApiGoogle {
            public string URL { get; init; }
            public AppSettingsApiGoogleReCAPTCHA ReCAPTCHA { get; init; } public class AppSettingsApiGoogleReCAPTCHA {
                public string Prefix { get; init; }
            }
        }
    }
    public static AppSettingsApi Hub { get; private set; } public class AppSettingsHub {
        public AppSettingsHubBase Base { get; init; } public class AppSettingsHubBase {
            public string URL { get; init; }
            public string Prefix { get; init; }
        }
    }
    public static AppSettingsLanguage Language { get; private set; } public class AppSettingsLanguage {
        public bool UsePrefix { get; init; }
        public string[] Hosts { get; init; }
        public string[] Languages { get; init; }
        public string DefaultLanguage { get; init; }
    }
    public static AppSettingsTheme Theme { get; private set; } public class AppSettingsTheme {
        public bool AutoDetect { get; init; }
    }
    public static AppSettingsGame Game { get; private set; } public class AppSettingsGame {
        public int FPS { get; init; }
        public AppSettingsGameRound Round { get; init; } public class AppSettingsGameRound {
            public int Minutes { get; init; }
        }
        public AppSettingsGameFinishDelay FinishDelay { get; init; } public class AppSettingsGameFinishDelay {
            public int Seconds { get; init; }
        }
        public int MaxSpectators { get; init; }
        public int MaxInstances { get; init; }
        public AppSettingsGameTouchDeviceNotifications TouchDeviceNotifications { get; init; }
        public class AppSettingsGameTouchDeviceNotifications {
            public int PerSecond { get; init; }
        }
        public AppSettingsGamePingInterval PingInterval { get; init; } public class AppSettingsGamePingInterval {
            public int Seconds { get; init; }
        }
    }
    public static AppSettingsLinks Links { get; private set; } public class AppSettingsLinks {
        public string Adminer { get; init; }
        public string Email { get; init; }
    }
    public static AppSettingsReCAPTCHA ReCAPTCHA { get; private set; } public class AppSettingsReCAPTCHA {
        public string SiteKey { get; init; }
        public bool On { get; init; }
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(IConfiguration config, IConfiguration file) {
        InitOnce.Check(nameof(AppSettings));
        Name = file.GetValue<string>("Name")!;
        Email = file.GetValue<string>("Email")!;
        Version = file.GetValue<string>("Version")!;
        Prerender = file.GetValue<bool>("Prerender")!;
        Redirect = file.GetValue<bool>("Redirect")!;
        Bundle = file.GetValue<bool>("Bundle")!;
        Api = new() {
            Base = new() {
                URL = file.GetValue<string>("Api:Base:URL")!,
                Prefix = file.GetValue<string>("Api:Base:Prefix")!
            },
            Google = new() {
                URL = file.GetValue<string>("Api:Google:URL")!,
                ReCAPTCHA = new() {
                    Prefix = file.GetValue<string>("Api:Google:ReCAPTCHA:Prefix")!
                }
            }
        };
        Hub = new() {
            Base = new() {
                URL = file.GetValue<string>("Hub:Base:URL")!,
                Prefix = file.GetValue<string>("Hub:Base:Prefix")!
            }
        };
        Language = new() {
            UsePrefix = file.GetValue<bool>("Language:UsePrefix"),
            Hosts = file.GetSection("Language:Hosts").Get<string[]>()!,
            Languages = file.GetSection("Language:Languages").Get<string[]>()!,
            DefaultLanguage = file.GetValue<string>("Language:DefaultLanguage")!
        };
        Theme = new() {
            AutoDetect = file.GetValue<bool>("Theme:AutoDetect")
        };
        Game = new() {
            FPS = file.GetValue<int>("Game:FPS")!,
            Round = new() { Minutes = file.GetValue<int>("Game:Round:Minutes")! },
            FinishDelay = new() { Seconds = file.GetValue<int>("Game:FinishDelay:Seconds")! },
            MaxSpectators = file.GetValue<int>("Game:MaxSpectators")!,
            MaxInstances = file.GetValue<int>("Game:MaxInstances")!,
            TouchDeviceNotifications = new() { PerSecond = file.GetValue<int>("Game:TouchDeviceNotifications:PerSecond")! },
            PingInterval = new() { Seconds = file.GetValue<int>("Game:PingInterval:Seconds")! }
        };
        Links = new() {
            Adminer = file.GetValue<string>("Links:Adminer")!,
            Email = file.GetValue<string>("Links:Email")!
        };
        ReCAPTCHA = new() {
            SiteKey = file.GetValue<string>("ReCAPTCHA:SiteKey")!,
            On = file.GetValue<bool>("ReCAPTCHA:On")!
        };
    }
}
