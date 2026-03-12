namespace Jumpeno.Server.Services;

public class CookieStorageServer : CookieStorage
{
    protected override void DeleteItem(string key, string domain, string path)
    {
        var ctx = ServerContext.Instance;
        ctx.Response.Cookies.Delete(
            key,
            new CookieOptions
            {
                Domain = Cookie.NormDomain(domain),
                Path = path,
            }
        );
    }

    protected override string? GetItem(string key)
    {
        var ctx = ServerContext.Instance;
        ctx.Request.Cookies.TryGetValue(key, out string? cookie);
        return cookie;
    }

    protected override void SetItem(Cookie cookie)
    {
        var ctx = ServerContext.Instance;
        ctx.Response.Cookies.Append(
            cookie.Key.String(),
            cookie.Value,
            new CookieOptions
            {
                Expires = cookie.Expires,
                Domain = Cookie.NormDomain(cookie.Domain),
                Path = cookie.Path,
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                SameSite = (Microsoft.AspNetCore.Http.SameSiteMode)(int)cookie.SameSite
            }
        );
    }
}
