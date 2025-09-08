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

    // Types ------------------------------------------------------------------------------------------------------------------------------
    private static bool IsCookieType<Type>(Type keyType) {
        if (keyType is null) return false;
        foreach (var cookieType in COOKIE.TYPES) {
            if (keyType.Equals(cookieType)) return true;
        }
        return false;
    }

    private static List<Type> ConvertToTypes(List<string> acceptedNames) {
        List<Type> accepted = [];
        foreach (var name in acceptedNames) {
            Type? type;
            try { type = Type.GetType($"{typeof(COOKIE).FullName}+{name}"); }
            catch { continue; }
            if (type is null) continue;
            try { if (IsCookieType(type)) accepted.Add(type); }
            catch { continue; }
        }
        return accepted;
    }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    private static bool IsAcceptedBy(Enum key, List<Type> accepted) {
        return accepted.Contains(key.GetType());
    }
    private static bool AreAcceptedBy(Enum[] keys, List<Type> accepted) {
        foreach (var key in keys) {
            if (!IsAcceptedBy(key, accepted)) return false;
        }
        return true;
    }
    // General:
    private static bool IsAccepted(Enum key) => AreAccepted([key]);
    private static bool AreAccepted(Enum[] keys) {
        var allRequired = AreAcceptedBy(keys, COOKIE.TYPES_REQUIRED);
        if (allRequired) return true;

        var accepted = GetAcceptedCookies();
        if (accepted.Count == 0) {
            accepted = COOKIE.TYPES_REQUIRED;
        }
        return AreAcceptedBy(keys, accepted);
    }

    // Accepted ---------------------------------------------------------------------------------------------------------------------------
    public static List<Type> GetAcceptedCookies() {
        var json = GetCookie(COOKIE.MANDATORY.APP_COOKIES_ACCEPTED);
        if (json is null) return [];
        
        var acceptedNames = JsonConvert.DeserializeObject<List<string>>(json);
        if (acceptedNames is null) return [];

        return ConvertToTypes(acceptedNames);
    }

    private static void SetAcceptedCookies(List<Type> accepted) {
        List<string> names = accepted.Select(x => x.Name).ToList();
        if (names.Count <= 0) {
            DeleteCookie(COOKIE.MANDATORY.APP_COOKIES_ACCEPTED);
        } else {
            var json = JsonConvert.SerializeObject(names);
            SetCookie(new Cookie(
                COOKIE.MANDATORY.APP_COOKIES_ACCEPTED,
                json,
                DateTimeOffset.UtcNow.AddYears(1)
            ));
        }
    }

    // Cookie modal -----------------------------------------------------------------------------------------------------------------------
    private static bool ModalInitialized = false;
    public static bool ModalOpenOnInit { get; private set; }

    public static async Task<bool> InitModal() {
        if (ModalInitialized) throw new InvalidOperationException("Already initialized!");
        else ModalInitialized = true;
        var accepted = await HTTP.Sync(GetAcceptedCookies);
        ModalOpenOnInit = accepted.Count <= 0;
        if (ModalOpenOnInit) await OpenConsentModal(true);
        return ModalOpenOnInit;
    }

    public static async Task OpenModal() => await OpenConsentModal(false);

    // Cookie consent ---------------------------------------------------------------------------------------------------------------------
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
            foreach (Enum cookie in Enum.GetValues(type)) {
                COOKIE.ORIGIN.TryGetValue(cookie, out var origins);
                if (origins == null) {
                    DeleteCookie(cookie);
                } else {
                    foreach (var (DOMAIN, PATH) in origins) {
                        DeleteCookie(cookie, DOMAIN, PATH);
                    }
                }
            }
        }
    }

    public static void AcceptCookieConsent(List<string> accept) => AcceptCookieConsent(ConvertToTypes(accept));

    // Cookie usage -----------------------------------------------------------------------------------------------------------------------
    private static string? GetCookie(Enum key) {
        var keyValue = key.String();
        Checker.CheckEmptyString(keyValue, name: "key");
        return GetItem(keyValue);
    }
    public static string? Get(Enum key) {
        if (!IsAccepted(key)) return null;
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
        if (!IsAccepted(cookie.Key)) return;
        SetCookie(cookie);
    }

    private static void DeleteCookie(Enum key, string? domain = null, string? path = null) {
        var keyValue = key.String();
        Checker.CheckEmptyString(keyValue, name: "key");
        DeleteItem(
            keyValue,
            domain is null ? COOKIE.DEFAULT_DOMAIN : domain,
            path is null ? COOKIE.DEFAULT_PATH : path
        );
    }
    public static void Delete(Enum key, string? domain = null, string? path = null) {
        if (!IsAccepted(key)) return;
        DeleteCookie(key, domain, path);
    }

    private static async Task ExecWithCookie(Enum[] cookieType, EmptyDelegate callback) {
        if (!AreAccepted(cookieType)) return;
        await callback.Invoke();
    }
    public static async Task WithCookie(Enum[] cookieType, Func<Task> callback) {
        await ExecWithCookie(cookieType, new(callback));
    }
    public static async Task WithCookie(Enum[] cookieType, Action callback) {
        await ExecWithCookie(cookieType, new(callback));
    }
    public static async Task WithCookie(Enum cookieType, Func<Task> callback) {
        await ExecWithCookie([cookieType], new(callback));
    }
    public static async Task WithCookie(Enum cookieType, Action callback) {
        await ExecWithCookie([cookieType], new(callback));
    }
}
