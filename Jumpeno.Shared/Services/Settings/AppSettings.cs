namespace Jumpeno.Shared.Services;

#pragma warning disable CS8618

public static class AppSettings {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static string Name { get; private set; }
    public static string Version { get; private set; }
    public static bool Prerender { get; private set; }
    public static bool Bundle { get; private set; }
    public static AppSettingsApi Api { get; private set; } public class AppSettingsApi {
        public AppSettingsApiBase Base { get; init; } public class AppSettingsApiBase {
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
    public static AppSettingsHub Hub { get; private set; } public class AppSettingsHub {
        public AppSettingsHubGame Game { get; init; } public class AppSettingsHubGame {
            public string URL { get; init; }
        }
    }
    public static AppSettingsGame Game { get; private set; } public class AppSettingsGame {
        public int FPS { get; init; }
        public AppSettingsGameTouchDeviceNotifications TouchDeviceNotifications { get; init; }
        public class AppSettingsGameTouchDeviceNotifications {
            public int PerSecond { get; init; }
        }
        public AppSettingsGameRound Round { get; init; } public class AppSettingsGameRound {
            public int Minutes { get; init; }
        }
        public AppSettingsGameFinishDelay FinishDelay { get; init; } public class AppSettingsGameFinishDelay {
            public int Seconds { get; init; }
        }
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(IConfiguration config) {
        Name = config.GetValue<string>("Name")!;
        Version = config.GetValue<string>("Version")!;
        Prerender = config.GetValue<bool>("Prerender")!;
        Bundle = config.GetValue<bool>("Bundle")!;
        Api = new() {
            Base = new() {
                URL = config.GetValue<string>("Api:Base:URL")!,
                Prefix = config.GetValue<string>("Api:Base:Prefix")!
            }
        };
        Language = new() {
            UsePrefix = config.GetValue<bool>("Language:UsePrefix"),
            Hosts = config.GetSection("Language:Hosts").Get<string[]>()!,
            Languages = config.GetSection("Language:Languages").Get<string[]>()!,
            DefaultLanguage = config.GetValue<string>("Language:DefaultLanguage")!
        };
        Theme = new() {
            AutoDetect = config.GetValue<bool>("Theme:AutoDetect")
        };
        Hub = new() {
            Game = new() {
                URL = config.GetValue<string>("Hub:Game:URL")!
            }
        };
        Game = new() {
            FPS = config.GetValue<int>("Game:FPS")!,
            TouchDeviceNotifications = new() { PerSecond = config.GetValue<int>("Game:TouchDeviceNotifications:PerSecond")! },
            Round = new() { Minutes = config.GetValue<int>("Game:Round:Minutes")! },
            FinishDelay = new() { Seconds = config.GetValue<int>("Game:FinishDelay:Seconds")! }
        };
    }
}
