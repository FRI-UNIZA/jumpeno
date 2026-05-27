namespace Jumpeno.Client.Services;

public class CookieStorageClient : CookieStorage
{
    protected override void DeleteItem(string key, string domain, string path)
    {
        HTTP.EnforceSync();
        JS.InvokeVoid(JSCookies.Delete, key, Cookie.NormDomain(domain), path);
    }

    protected override string? GetItem(string key)
    {
        HTTP.EnforceSync();
        var value = JS.Invoke<string>(JSCookies.Get, key);
        if (value is null) return value;
        else return URL.DecodeValue(value);
    }

    protected override void SetItem(Cookie cookie)
    {
        HTTP.EnforceSync();
        JS.InvokeVoid(
            JSCookies.Set,
            cookie.Key.String(),
            URL.EncodeValue(cookie.Value),
            cookie.Expires is not null ? ((DateTimeOffset)cookie.Expires).UtcDateTime.ToString("R") : null,
            cookie.Domain == Cookie.NormDomain(cookie.Domain),
            cookie.Path,
            cookie.Secure,
            cookie.SameSite == SameSite.Unspecified ? null : cookie.SameSite.String()
        );
    }
}
