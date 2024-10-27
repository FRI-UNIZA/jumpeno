namespace Jumpeno.Shared.Services;

#pragma warning disable CS8618

public static class AppSettings {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static string Name { get; private set; }
    public static string Version { get; private set; }
    public static bool Prerender { get; private set; }
    public static ApiSettings Api { get; private set; }
    public static LanguageSettings Language { get; private set; }
    public static ThemeSettings Theme { get; private set; }
    public static HubSettings Hub { get; private set; }

    // Initializer ------------------------------------------------------------------------------------------------------------------------
    public static void Init(IConfiguration config) {
        Name = config.GetValue<string>("Name")!;
        Version = config.GetValue<string>("Version")!;
        Prerender = config.GetValue<bool>("Prerender")!;
        Api = new ApiSettings(
            new ApiBaseSettings(
                config.GetValue<string>("Api:Base:URL")!,
                config.GetValue<string>("Api:Base:Prefix")!
            )
        );
        Language = new LanguageSettings(
            config.GetValue<bool>("Language:UsePrefix"),
            config.GetSection("Language:Hosts").Get<string[]>()!,
            config.GetSection("Language:Languages").Get<string[]>()!,
            config.GetValue<string>("Language:DefaultLanguage")!
        );
        Theme = new ThemeSettings(
            config.GetValue<bool>("Theme:AutoDetect")
        );
        Hub = new HubSettings(
            new GameHubSettings(
                config.GetValue<string>("Hub:Game:URL")!
            )
        );
    }
}

// ApiSettings ----------------------------------------------------------------------------------------------------------------------------
public record ApiSettings(
    ApiBaseSettings Base
);

public record ApiBaseSettings(
    string URL,
    string Prefix
);
// END ApiSettings ------------------------------------------------------------------------------------------------------------------------

// LanguageSettings ----------------------------------------------------------------------------------------------------------------------- 
public record LanguageSettings(
    bool UsePrefix,
    string[] Hosts,
    string[] Languages,
    string DefaultLanguage
);
// END LanguageSettings -------------------------------------------------------------------------------------------------------------------

// ThemeSettings --------------------------------------------------------------------------------------------------------------------------
public record ThemeSettings(
    bool AutoDetect
);
// END ThemeSettings ----------------------------------------------------------------------------------------------------------------------

// HubSettings ----------------------------------------------------------------------------------------------------------------------------
public record HubSettings(
    GameHubSettings Game
);

public record GameHubSettings(
    string URL
);
// END HubSettings ------------------------------------------------------------------------------------------------------------------------
