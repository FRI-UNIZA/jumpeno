namespace Jumpeno.Shared.Services;

using Newtonsoft.Json;

#pragma warning disable CS8618

public static class CookieStorage {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static Func<string, string?> GetItem;
    private static Action<Cookie> SetItem;
    private static Action<string, string, string> DeleteItem;
    private static Func<bool, Task> OpenConsentModal;

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(
        Func<string, string?> getItem,  Action<Cookie> setItem, Action<string, string, string> deleteItem,
        Func<bool, Task> openConsentModal
    ) {
        if (GetItem is not null) throw new InvalidOperationException("Already initialized!");
        GetItem = getItem;
        SetItem = setItem;
        DeleteItem = deleteItem;
        OpenConsentModal = openConsentModal;
    }

    // Cookie types -----------------------------------------------------------------------------------------------------------------------
    public static List<Type> ConvertToTypes(List<string> acceptedNames) {
        List<Type> accepted = [];
        foreach (var name in acceptedNames) {
            Type? type;
            try { type = Type.GetType($"{COOKIE.NAMESPACE}.{name}"); }
            catch { type = null; }
            if (type is null) continue;
            try { if (IsCookieType(type)) accepted.Add(type); }
            catch { continue; }
        }
        return accepted;
    }

    public static List<Type> CacheAcceptedCookies(List<Type> accepted) {
        RequestStorage.Set(nameof(GetAcceptedCookies), accepted);
        return accepted;
    }

    public static List<Type> GetAcceptedCookies() {
        var cachedAccepted = RequestStorage.Get<List<Type>>(nameof(GetAcceptedCookies));
        if (cachedAccepted is not null) return cachedAccepted;

        var json = GetCookie(COOKIE_MANDATORY.APP_COOKIES_ACCEPTED);
        if (json is null) return CacheAcceptedCookies([]);
        
        var acceptedNames = JsonConvert.DeserializeObject<List<string>>(json);
        if (acceptedNames is null) return CacheAcceptedCookies([]);

        return CacheAcceptedCookies(ConvertToTypes(acceptedNames));
    }

    private static void SetAcceptedCookies(List<Type> accepted) {
        List<string> names = accepted.Select(x => x.Name).ToList();
        var json = JsonConvert.SerializeObject(names);
        SetCookie(new Cookie(
            COOKIE_MANDATORY.APP_COOKIES_ACCEPTED,
            json,
            DateTimeOffset.UtcNow.AddYears(1)
        ));
        CacheAcceptedCookies(accepted);
    }

    private static bool IsCookieType<Type>(Type keyType) {
        foreach (var cookieType in COOKIE.TYPES) {
            if (keyType is null) return false;
            if (keyType.Equals(cookieType)) return true;
        }
        return false;
    }

    // Accepted ---------------------------------------------------------------------------------------------------------------------------
    private static bool IsAcceptedBy(Enum key, List<Type> accepted) {
        return accepted.Contains(key.GetType());
    }
    private static bool AreAcceptedBy(Enum[] keys, List<Type> accepted) {
        foreach (var key in keys) {
            if (!IsAcceptedBy(key, accepted)) return false;
        }
        return true;
    }

    private static bool IsAccepted(Enum[] keys) {
        var allRequired = AreAcceptedBy(keys, COOKIE.TYPES_REQUIRED);
        if (allRequired) return true;

        var accepted = GetAcceptedCookies();
        if (accepted.Count == 0) {
            accepted = COOKIE.TYPES_REQUIRED;
        }
        return AreAcceptedBy(keys, accepted);
    }

    // Cookie consent ---------------------------------------------------------------------------------------------------------------------    
    public static async Task<bool> InitModal() {
        var accepted = GetAcceptedCookies();
        if (accepted.Count > 0) {
            SetAcceptedCookies(accepted);
            return false;
        } else {
            await OpenConsentModal(true);
            return true;
        }
    }
    
    public static async Task OpenModal() => await OpenConsentModal(false);

    public static bool CookiesAccepted => GetAcceptedCookies().Count > 0;

    public static void AcceptCookieConsent(List<Type> accept) {
        List<Type> acceptedCookies = [];
        foreach (var accepted in accept) {
            if (IsCookieType(accepted)) {
                acceptedCookies.Add(accepted);
            }
        }
        SetAcceptedCookies(acceptedCookies);
        var unacceptedCookies = COOKIE.TYPES.Except(acceptedCookies);
        foreach (var type in unacceptedCookies) {
            foreach (var cookie in Enum.GetValues(type)) {
                var name = cookie.ToString() ?? "";
                COOKIE.DOMAIN.TryGetValue(name, out var domain);
                COOKIE.PATH.TryGetValue(name, out var path);
                DeleteCookie((Enum)cookie, domain, path);
            }
        }
    }
    
    // Cookie usage -----------------------------------------------------------------------------------------------------------------------
    private static string? GetCookie(Enum key) {
        var keyValue = key.String();
        Checker.CheckEmptyString(keyValue, name: "key");
        return GetItem(keyValue);
    }
    public static string? Get(Enum key) {
        if (!IsAccepted([key])) return null;
        return GetCookie(key);
    }

    private static void SetCookie(Cookie cookie) {
        Checker.CheckEmptyString(cookie.Key.String(), name: "key");
        if (!AppEnvironment.IsServer && cookie.HttpOnly) {
            cookie.HttpOnly = false;
        }
        if (!cookie.Secure && cookie.SameSite == SAME_SITE.NONE) {
            throw new Exception("Only secure cookies can have \"SameSite: None\"");
        }
        cookie.Value = cookie.Value;
        SetItem(cookie);
    }
    public static void Set(Cookie cookie) {
        if (!IsAccepted([cookie.Key])) return;
        SetCookie(cookie);
    }

    private static void DeleteCookie(Enum key, string? domain = null, string? path = null) {
        var keyValue = key.String();
        Checker.CheckEmptyString(keyValue, name: "key");
        DeleteItem(
            keyValue,
            domain is null ? Cookie.DEFAULT_DOMAIN : domain,
            path is null ? Cookie.DEFAULT_PATH : path
        );
    }
    public static void Delete(Enum key, string? domain = null, string? path = null) {
        if (!IsAccepted([key])) return;
        DeleteCookie(key, domain, path);
    }

    private static async Task ExecWithCookie(Enum[] cookieType, EmptyDelegate callback) {
        foreach (var type in cookieType) {
            if (!IsAccepted([type])) return;
        }
        await callback.Invoke();
    }
    public static async Task WithCookie(Enum[] cookieType, Func<Task> callback) {
        await ExecWithCookie(cookieType, new(callback));
    }
    public static async Task WithCookie(Enum[] cookieType, Action callback) {
        await ExecWithCookie(cookieType, new(callback));
    }
}
