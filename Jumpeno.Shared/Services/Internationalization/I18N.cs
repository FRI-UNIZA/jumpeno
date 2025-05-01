namespace Jumpeno.Shared.Services;

#pragma warning disable CS8618

using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Localization;

public class I18N {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private static IStringLocalizer<Resource> Localizer;
    public static bool USE_PREFIX { get; private set; }
    public static string [] HOSTS { get; private set; }
    public static string[] LANGUAGES { get; private set; }
    public static string FALLBACK { get; private set; }
    private static Dictionary<string, string> LanguageHost;
    private static Dictionary<string, string> HostLanguage;
    private const string ESCAPE_START = "I18N{";
    private const string ESCAPE_END = "}";
    private const string SPLIT = "@I18N_SPLIT{}";

    // Initializer ------------------------------------------------------------------------------------------------------------------------
    public static void Init(IStringLocalizer<Resource> localizer) {
        if (Localizer is not null) {
            throw new Exception("I18N already initialized!");
        }
        Localizer = localizer;
        USE_PREFIX = AppSettings.Language.UsePrefix;
        HOSTS = AppSettings.Language.Hosts;
        LANGUAGES = AppSettings.Language.Languages;
        FALLBACK = AppSettings.Language.DefaultLanguage;
        LanguageHost = [];
        HostLanguage = [];
        for (var i = 0; i < HOSTS.Length; i++) {
            LanguageHost.Add(LANGUAGES[i], HOSTS[i]);
            HostLanguage.Add(HOSTS[i], LANGUAGES[i]);
        }
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void SetCulture(string culture) {
        var cultureInfo = new CultureInfo(culture);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }

    public static string Culture => CultureInfo.CurrentCulture.ToString();

    public static bool IsCulture(LANGUAGE language) {
        return Culture == language.String();
    }

    public static string GetHost(string language) {
        return LanguageHost[language];
    }

    public static string GetLanguage(string host) {
        return HostLanguage[host];
    }

    public static bool IsCurrentCultureUrl(string url) {
        if (AppSettings.Language.UsePrefix) {
            var path = URL.Path(url, keepEnd: true);
            return URL.IsLocal(url) && (
                path.StartsWith($"/{Culture}/")
                || path == $"/{Culture}"
            );
        } else {
            var host = URL.Host(url);
            return host == "" || host == GetHost(Culture);
        }
    }

    // Translations -----------------------------------------------------------------------------------------------------------------------
    private static string T(string key, Dictionary<string, string>? values = null) {
        if (values is null) {
            return Localizer[key];
        }
        try {
            string result = Localizer[key];

            do {
                var index = result.IndexOf(ESCAPE_START);
                if (index < 0) break;
                int end = result.IndexOf(ESCAPE_END, index);

                string name = result.Substring(index + ESCAPE_START.Length, end - (index + ESCAPE_START.Length));
                result = result.Replace($"{ESCAPE_START}{name}{ESCAPE_END}", values[name]);
            } while (true);

            return result;
        } catch {
            return Localizer[key];
        }
    }
    public static string T(string key, Dictionary<string, string>? values = null, bool unsplit = false) {
        if (unsplit) return UnSplit(T(key, values));
        else return T(key, values);
    }

    public static string[] Split(string value) => value.Split(SPLIT);
    public static string UnSplit(string value) => value.Replace(SPLIT, "");

    // Links ------------------------------------------------------------------------------------------------------------------------------
    private static string PageLink<T>() {
        string link = typeof(T).GetField($"ROUTE_{Culture.ToUpper()}")!.GetValue(null)!.ToString()!;
        if (USE_PREFIX && link == $"/{Culture}") {
            return $"{link}/";
        }
        if (link.EndsWith('/')) {
            return link[..^1];
        }
        return link;
    }

    public static string Link<T>() {
        string link = URL.RemoveSegments(PageLink<T>());
        return URL.Encode(link);
    }
    public static string Link<T>(object[] parameters) {
        var link = URL.Encode(PageLink<T>());
        if (USE_PREFIX && link.StartsWith($"/{Culture}")) {
            link = link[$"/{Culture}".Length..];
        }
        var linkParams = parameters.Prepend(link).ToArray();

        // Call Link method of page:
        MethodInfo? method;
        try { method = typeof(T).GetMethod("Link", BindingFlags.Static | BindingFlags.Public); }
        catch { throw new Exception("Static Link method not implemented in page!"); }
        link = (string) method!.Invoke(null, linkParams)!;

        if (link.EndsWith('/')) link = link[..^1];
        
        return USE_PREFIX ? $"/{URL.EncodeValue(Culture)}{link}" : link;
    }
}
