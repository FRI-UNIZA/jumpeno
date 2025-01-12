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
    }
}
