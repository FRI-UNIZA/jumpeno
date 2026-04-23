namespace Jumpeno.Client.Services;

#pragma warning disable CS8618

public class I18N {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private static IStringLocalizer<Resource> Localizer;
    public static bool UsePrefix { get; private set; }
    public static string [] Hosts { get; private set; }
    public static string[] Languages { get; private set; }
    public static string Fallback { get; private set; }
    private static Dictionary<string, string> LanguageHost;
    private static Dictionary<string, string> HostLanguage;
    private const string EscapeStart = "I18N{";
    private const string EscapeEnd = "}";
    private const string SplitSeparator = "@I18N_SPLIT{}";

    // Initializer ------------------------------------------------------------------------------------------------------------------------
    public static void Init(IStringLocalizer<Resource> localizer) {
        InitOnce.Check(nameof(I18N));
        Localizer = localizer;
        UsePrefix = AppSettings.Language.UsePrefix;
        Hosts = AppSettings.Language.Hosts;
        Languages = AppSettings.Language.Languages;
        Fallback = AppSettings.Language.DefaultLanguage;
        LanguageHost = [];
        HostLanguage = [];
        for (var i = 0; i < Hosts.Length; i++) {
            LanguageHost.Add(Languages[i], Hosts[i]);
            HostLanguage.Add(Hosts[i], Languages[i]);
        }
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void SetCulture(string culture) {
        var cultureInfo = new CultureInfo(culture);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }

    public static string Culture => CultureInfo.CurrentCulture.ToString();

    public static bool IsCulture(Language language) {
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
    private static string T(string key, Dictionary<string, object>? values = null) {
        if (values is null || values.Count == 0) return Localizer[key];
        try {
            string result = Localizer[key];
            do {
                var index = result.IndexOf(EscapeStart);
                if (index < 0) break;
                int end = result.IndexOf(EscapeEnd, index);

                string name = result.Substring(index + EscapeStart.Length, end - (index + EscapeStart.Length));
                result = result.Replace($"{EscapeStart}{name}{EscapeEnd}", $"{values[name]}");
            } while (true);

            return result;
        } catch {
            return Localizer[key];
        }
    }
    public static string T(string key, Dictionary<string, object>? values = null, bool unsplit = false) {
        if (unsplit) return UnSplit(T(key, values));
        else return T(key, values);
    }
    public static string T(TInfo message, bool unsplit = false) => T(message.Key, message.Values, unsplit);

    public static string[] Split(string value) => value.Split(SplitSeparator);
    public static string UnSplit(string value) => value.Replace(SplitSeparator, "");

    // Links ------------------------------------------------------------------------------------------------------------------------------
    private static string PageLink<T>() {
        string link = typeof(T).GetField($"ROUTE_{Culture.ToUpper()}")!.GetValue(null)!.ToString()!;
        if (UsePrefix && link == $"/{Culture}") {
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
        if (UsePrefix && link.StartsWith($"/{Culture}")) {
            link = link[$"/{Culture}".Length..];
        }
        var linkParams = parameters.Prepend(link).ToArray();

        // Call Link method of page:
        MethodInfo? method;
        try { method = typeof(T).GetMethod("Link", BindingFlags.Static | BindingFlags.Public); }
        catch { throw new Exception("Static Link method not implemented in page!"); }
        link = (string) method!.Invoke(null, linkParams)!;

        if (link.EndsWith('/')) link = link[..^1];
        
        return UsePrefix ? $"/{URL.EncodeValue(Culture)}{link}" : link;
    }
}
