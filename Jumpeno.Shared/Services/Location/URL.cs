namespace Jumpeno.Shared.Services;

#pragma warning disable CS8618

public static class URL {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string SCHEMA_DIVIDER = "://";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static Func<string> ThemeCSSClass;

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(Func<string> url, Func<string> themeCSSClass) {
        if (Url is not null) throw new InvalidOperationException("Already initialized!");
        Url = url;
        ThemeCSSClass = themeCSSClass;
    }

    // Normalization ----------------------------------------------------------------------------------------------------------------------
    public static string NormSchema(string schema) {
        if (schema.EndsWith(SCHEMA_DIVIDER)) return schema;
        return $"{schema}{SCHEMA_DIVIDER}";
    }
    public static string NormBaseUrl(string baseUrl) {
        if (baseUrl.EndsWith('/')) return baseUrl[..^1];
        return baseUrl;
    }
    public static string NormPath(string path, bool keepEnd = false) {
        if (!path.StartsWith('/')) path = $"/{path}";
        if (!keepEnd && path != "/" && path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
        return path;
    }

    // Current URL methods ----------------------------------------------------------------------------------------------------------------
    public static Func<string> Url { get; private set; }
    public static string Schema() => Schema(Url());
    public static string Host() => Host(Url());
    public static string Port() => Port(Url());
    public static string Domain() => Domain(Url());
    public static string BaseUrl() => BaseUrl(Url());
    public static string Path(bool keepEnd = false) => Path(Url(), keepEnd);
    public static string Query() => Query(Url());
    public static string ToAbsolute(string url) => ToAbsolute(BaseUrl(), url);
    public static string ToAbsolute() => ToAbsolute(BaseUrl(), Url());
    public static string ToRelative() => ToRelative(Url());
    public static string NoQuery() => NoQuery(Url());
    public static string WithQuery(string query) => WithQuery(Url(), query);
    public static QueryParams GetQueryParams(Dictionary<string, QUERY_ARRAY_TYPE> arrayTypes) => GetQueryParams(arrayTypes, Url());
    public static QueryParams GetQueryParams() => GetQueryParams(Url());
    public static string SetQueryParams(QueryParams queryParams) => SetQueryParams(Url(), queryParams);
    public static bool PathMatches(string url, bool exact = false) => PathMatches(Url(), url, exact);
    public static string ReplaceSegments(Dictionary<int, string> segments) => ReplaceSegments(Url(), segments);

    // Custom URL methods -----------------------------------------------------------------------------------------------------------------
    public static string Schema(string url) {
        var index = url.IndexOf(SCHEMA_DIVIDER);
        if (index >= 0) {
            return url.Substring(0, index);
        }
        return "";
    }

    public static string Host(string url) {
        var schema = Schema(url);
        if (schema != "" || url.StartsWith(SCHEMA_DIVIDER)) {
            url = url.Substring(NormSchema(schema).Length);
        }
        var index = url.IndexOf('/');
        if (index >= 0) url = url.Substring(0, index);
        return url;
    }

    public static string Port(string url) {
        var host = Host(url);
        var index = host.IndexOf(':');
        if (index < 0) return "";
        return host.Substring(index + 1);
    }

    public static string Domain(string url) {
        return Host(url).Replace($":{Port(url)}", "");;
    }

    public static string BaseUrl(string url) {
        var schema = Schema(url);
        var host = Host(url);
        if (schema == "" || host == "") return "";
        return $"{NormSchema(schema)}{host}";
    }

    public static string Path(string url, bool keepEnd = false) {
        var baseUrl = BaseUrl(url);
        var query = Query(url);
        if (baseUrl != "") {
            url = url.Substring(baseUrl.Length);
        }
        if (query != "") {
            url = url.Substring(0, url.Length - query.Length);
        }
        return NormPath(url, keepEnd);
    }

    public static string Query(string url) {
        var index = url.IndexOf('?');
        if (index >= 0 && index > url.LastIndexOf("?}")) {
            url = url.Substring(index);
            if (url.IndexOf('?') < 0 || url.IndexOf("=") < 0) {
                return "";
            }
            return url;
        }
        return "";
    }

    public static string ToAbsolute(string baseUrl, string url) {
        if (BaseUrl(url) != "") return url;
        baseUrl = NormBaseUrl(baseUrl);
        url = url == "/" || url == "" ? "" : NormPath(url, true);
        return $"{baseUrl}{url}";
    }

    public static string ToRelative(string url) {
        var baseUrl = BaseUrl(url);
        if (baseUrl == "") return NormPath(url, true);
        return NormPath(url.Substring(baseUrl.Length), true);
    }

    public static string NoQuery(string url) {
        return url.Substring(0, url.Length - Query(url).Length);
    }

    public static string WithQuery(string url, string query) {
        return $"{NoQuery(url)}{query}";
    }

    public static QueryParams GetQueryParams(Dictionary<string, QUERY_ARRAY_TYPE> arrayTypes, string url) {
        return new QueryParams(arrayTypes, QueryHelpers.ParseQuery(Query(url)));
    }
    public static QueryParams GetQueryParams(string url) {
        return GetQueryParams([], url);
    }

    public static string SetQueryParams(string url, QueryParams queryParams) {
        return WithQuery(NoQuery(url), queryParams.ToString());
    }

    public static bool IsLocal(string url) {
        var baseURL = BaseUrl(url);
        return baseURL == "" || baseURL == BaseUrl();
    }

    public static bool PathMatches(string url1, string url2, bool exact = false) {
        var path1 = Path(url1).ToLower();
        var path2 = Path(url2).ToLower();
       return exact ? path1 == path2 : path2.StartsWith(path1) || path1.StartsWith(path2);
    }

    // Segments ---------------------------------------------------------------------------------------------------------------------------
    public static string? GetSegment(string url, int index, bool noCulture = false, bool decode = false) {
        string? segment;
        try {
            var path = Path(url).Substring(1);
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            segment = segments[noCulture ? index + 1 : index];
        } catch {
            segment = null;
            return segment;
        }
        return decode ? Decode(segment) : segment;
    }
    public static string ReplaceSegments(string url, Dictionary<int, string> segments, bool encode = false) {
        var baseUrl = BaseUrl(url);
        var path = Path(url);
        var query = Query(url);
        var oldSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var newPath = "";
        for (var i = 0; i < oldSegments.Length; i++) {
            var segment = segments.ContainsKey(i)
                          ? encode ? EncodeValue(segments[i]) : segments[i]
                          : oldSegments[i];
            newPath = $"{newPath}/{segment}";
        }
        if (newPath == "") newPath = "/";

        return $"{baseUrl}{newPath}{query}";
    }
    public static string RemoveSegments(string url) {
        do {
            var index = url.IndexOf("/{");
            if (index < 0) break;
            var lastIndex = url.IndexOf('}', index);
            if (lastIndex < 0) break;
            url = url.Remove(index, lastIndex - index + 1);
        } while (true);
        return url;
    }

    // Encoding ---------------------------------------------------------------------------------------------------------------------------
    public static string EncodeValue(string value) {
        return Uri.EscapeDataString(value);
    }
    public static string DecodeValue(string value) {
        return Uri.UnescapeDataString(value);
    }
    private static string Encode(string url, bool encode) {
        var baseUrl = BaseUrl(url);
        var path = Path(url, keepEnd: true);
        var query = Query(url);

        var oldSegments = path.Substring(1).Split('/');
        var newPath = "";
        for (var i = 0; i < oldSegments.Length; i++) {
            var segment = encode ? EncodeValue(oldSegments[i]) : DecodeValue(oldSegments[i]);
            newPath = $"{newPath}/{segment}";
        }
        if (newPath == "") newPath = "/";

        oldSegments = query.Split('&', StringSplitOptions.RemoveEmptyEntries);
        var newQuery = oldSegments.Length > 0 ? "?" : "";
        for (var i = 0; i < oldSegments.Length; i++) {
            var segment = i == 0 ? oldSegments[i].Substring(1) : oldSegments[i];
            var parts = segment.Split('=');

            if (parts.Length != 2) continue;

            parts[0] = i == 0 ? parts[0] : $"&{parts[0]}";
            var qs = encode ? EncodeValue(parts[1]) : DecodeValue(parts[1]);

            newQuery = $"{newQuery}{parts[0]}={qs}";
        }

        return $"{baseUrl}{newPath}{newQuery}";
    }
    public static string Encode(string url) {
        return Encode(url, true);
    }
    public static string Decode(string url) {
        return Encode(url, false);
    }

    // File -------------------------------------------------------------------------------------------------------------------------------
    public static string Directory(string path) {
        var index = path.LastIndexOf('/');
        return index == -1 ? "" : path.Substring(0, index);
    }
    public static string FileName(string path) => System.IO.Path.GetFileNameWithoutExtension(path) ?? "";
    public static string Extension(string path, bool dot = false) {
        var index = path.LastIndexOf('.');
        return index == -1 || index == path.Length - 1 ? "" : path.Substring(dot ? index : index + 1);
    }

    // File name --------------------------------------------------------------------------------------------------------------------------
    public static string AppendText(string file, string text) => $"{Directory(file)}/{FileName(file)}-{text}{Extension(file, true)}";    
    public static string AppendTheme(string file, string? theme = null) => AppendText(file, theme ?? ThemeCSSClass());
    public static string AppendCulture(string file, string? culture = null) =>  AppendText(file, culture ?? I18N.Culture);

    // Links ------------------------------------------------------------------------------------------------------------------------------
    public static string Link(string url, bool encode = false) {
        if (encode) return Encode(url);
        else return url;
    }
    public static string ImageLink(string file, bool theme = false, bool culture = false) {
        if (theme) file = AppendTheme(file);
        if (culture) file = AppendCulture(file);
        return Encode($"/images/{file}?v={AppSettings.Version}");
    }
}
