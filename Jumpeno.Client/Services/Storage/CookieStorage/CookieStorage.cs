namespace Jumpeno.Client.Services;

using Newtonsoft.Json;

public abstract class CookieStorage {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public CookieStorage() {
        InitOnce.Check(nameof(CookieStorage));
    }

    // Types ------------------------------------------------------------------------------------------------------------------------------
    protected abstract string? GetItem(string key);
    protected abstract void SetItem(Models.Cookie cookie);
    protected abstract void DeleteItem(string key, string domain, string path);

    private bool IsCookieType(Type keyType)
    {
        if (keyType is null) return false;
        return Cookies.TYPES.Contains(keyType);
    }

    private List<Type> ConvertToTypes(List<string> acceptedNames) {
        List<Type> accepted = [];
        foreach (var name in acceptedNames) {
            try
            {
                Type? type = Type.GetType($"{typeof(Cookies).FullName}+{name}");
                if (type is not null && IsCookieType(type)) accepted.Add(type);
            }
            catch
            {
                continue;
            }
        }
        return accepted;
    }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    private bool IsAcceptedBy(Enum key, List<Type> accepted) {
        return accepted.Contains(key.GetType());
    }
    private bool AreAcceptedBy(Enum[] keys, List<Type> accepted) {
        return keys.All(key => IsAcceptedBy(key, accepted));
    }
    // General:
    private bool IsAccepted(Enum key) => AreAccepted([key]);
    private bool AreAccepted(Enum[] keys) {
        var allRequired = AreAcceptedBy(keys, Cookies.TYPES_REQUIRED);
        if (allRequired) return true;

        var accepted = GetAcceptedCookies();
        if (accepted.Count == 0) {
            accepted = Cookies.TYPES_REQUIRED;
        }
        return AreAcceptedBy(keys, accepted);
    }

    // Accepted ---------------------------------------------------------------------------------------------------------------------------
    public bool IsCookieAccepted(Type key) => GetAcceptedCookies().Any(x => x == key);

    public List<Type> GetAcceptedCookies() {
        var json = GetCookie(Cookies.Mandatory.APP_COOKIES_ACCEPTED);
        if (json is null) return [];
        
        var acceptedNames = JsonConvert.DeserializeObject<List<string>>(json);
        if (acceptedNames is null) return [];

        return ConvertToTypes(acceptedNames);
    }

    private void SetAcceptedCookies(List<Type> accepted) {
        var names = accepted.Select(x => x.Name).ToList();
        if (names.Count <= 0) {
            DeleteCookie(Cookies.Mandatory.APP_COOKIES_ACCEPTED);
        } else {
            var json = JsonConvert.SerializeObject(names);
            SetCookie(new Models.Cookie(
                Cookies.Mandatory.APP_COOKIES_ACCEPTED,
                json,
                DateTimeOffset.UtcNow.AddYears(1)
            ));
        }
    }


    // Cookie consent ---------------------------------------------------------------------------------------------------------------------
    public void AcceptCookieConsent(List<Type> accept) {
        var acceptedCookies = accept.Where(IsCookieType).ToList();
        var unacceptedCookies = Cookies.TYPES.Except(acceptedCookies);

        SetAcceptedCookies(acceptedCookies);

        foreach (Enum cookie in unacceptedCookies.SelectMany(x => Enum.GetValues(x).Cast<Enum>())) {
            Cookies.ORIGIN.TryGetValue(cookie, out var origins);
            if (origins == null) {
                DeleteCookie(cookie);
            } else {
                foreach (var (DOMAIN, PATH) in origins) {
                    DeleteCookie(cookie, DOMAIN, PATH);
                }
            }
        }
    }

    public void AcceptCookieConsent(List<string> accept) => AcceptCookieConsent(ConvertToTypes(accept));

    // Cookie usage -----------------------------------------------------------------------------------------------------------------------
    private string? GetCookie(Enum key) {
        var keyValue = key.String();
        Checker.CheckEmptyString(keyValue, name: "key");
        return GetItem(keyValue);
    }
    public string? Get(Enum key) {
        if (!IsAccepted(key)) return null;
        return GetCookie(key);
    }

    private void SetCookie(Models.Cookie cookie) {
        Checker.CheckEmptyString(cookie.Key.String(), name: "key");
        if (!AppEnvironment.IsServer && cookie.HttpOnly) {
            cookie.HttpOnly = false;
        }
        if (!cookie.Secure && cookie.SameSite == SameSite.NONE) {
            throw new Exception("Only secure cookies can have \"SameSite: None\"");
        }
        cookie.Value = cookie.Value;
        SetItem(cookie);
    }
    public void Set(Models.Cookie cookie) {
        if (!IsAccepted(cookie.Key)) return;
        SetCookie(cookie);
    }

    private void DeleteCookie(Enum key, string? domain = null, string? path = null) {
        var keyValue = key.String();
        Checker.CheckEmptyString(keyValue, name: "key");
        DeleteItem(
            keyValue,
            domain is null ? Cookies.DEFAULT_DOMAIN : domain,
            path is null ? Cookies.DEFAULT_PATH : path
        );
    }
    public void Delete(Enum key, string? domain = null, string? path = null) {
        if (!IsAccepted(key)) return;
        DeleteCookie(key, domain, path);
    }

    private async Task<bool> ExecWithCookie(Enum[] cookieType, EmptyDelegate callback) {
        if (!AreAccepted(cookieType)) return false;
        await callback.Invoke();
        return true;
    }

    public Task WithCookie(Enum[] cookieType, Func<Task> callback) => ExecWithCookie(cookieType, new(callback));
    public Task WithCookie(Enum[] cookieType, Action callback) => ExecWithCookie(cookieType, new(callback));
    public Task WithCookie(Enum cookieType, Func<Task> callback) => ExecWithCookie([cookieType], new(callback));
    public Task WithCookie(Enum cookieType, Action callback) => ExecWithCookie([cookieType], new(callback));
}
