namespace Jumpeno.Server.Controllers;

using Microsoft.AspNetCore.Localization;
using System.Globalization;

[Route("[controller]/[action]")]
public class CultureController : Controller {
    [HttpGet]
    public IActionResult Set(string culture, string redirectUri) {
        if (culture != null) {
            CookieStorage.Set(new Cookie(
                COOKIE_FUNCTIONAL.APP_CULTURE,
                culture,
                DateTimeOffset.UtcNow.AddYears(1)
            ));
        }
        return Redirect(redirectUri);
    }

    public static Action<RequestLocalizationOptions> SetupAction() {
        return options => {
            var supportedCultures = I18N.LANGUAGES.Select(lang => new CultureInfo(lang)).ToList();
        
            options.DefaultRequestCulture = new RequestCulture(I18N.FALLBACK);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders.Clear();
            options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(
                async ctx => {
                    if (ServerEnvironment.IsStaticPath(ctx.Request.Path)) return null;
                    string culture = GetCultureString([.. I18N.LANGUAGES], I18N.FALLBACK, ctx);
                    UpdateURL(culture, ctx);
                    return await Task.FromResult(new ProviderCultureResult(culture));
                }
            ));
            options.ApplyCurrentCultureToResponseHeaders = true;
            options.CultureInfoUseUserOverride = false;
        };
    }

    public static string GetCultureString(List<string> languages, string defaultLanguage, HttpContext ctx) {    
        // (1) Language by domain name:
        if (!I18N.USE_PREFIX) {
            return I18N.GetLanguage(ctx.Request.Host.ToString());
        }

        // (2) Check path:
        string path = ctx.Request.Path;
        path = path[1..];
        int index = path.IndexOf('/');
        if (index > -1) {
            path = path[..path.IndexOf('/')];
        }
        if (languages.Contains(path)) {
            return path;
        }

        // (3) Check cookies:
        string? cookie = CookieStorage.Get(COOKIE_FUNCTIONAL.APP_CULTURE);
        cookie = cookie is not null ? cookie : "null";
        if (languages.Contains(cookie)) {
            return cookie;
        }

        // (4) Check user settings:
        string? acceptLanguage = ctx.Request.Headers.AcceptLanguage;
        if (acceptLanguage is null || acceptLanguage.Trim() == "") {
            return defaultLanguage;
        }
        var preferredLanguages = acceptLanguage.Split(",")
            .Select(StringWithQualityHeaderValue.Parse)
            .OrderByDescending(s => s.Quality.GetValueOrDefault(1))
            .ToArray();

        for (var i = 0; i < preferredLanguages.Count(); i++) {
            string language = preferredLanguages[i].Value;
            for (var j = 0; j < languages.Count; j++) {
                if (language.StartsWith(languages[j])) {
                    return languages[j];
                }
            }
        }

        // (5) No match, default language:
        return defaultLanguage;
    }

    public static void UpdateURL(string culture, HttpContext ctx) {
        string currentPath = URL.Path(keepEnd: true);
        string path = currentPath;

        if (!I18N.USE_PREFIX) {
            if (path != "/" && path.EndsWith('/')) {
                path = path[..^1];
            }
        } else {
            if (path == "/" || path == $"/{culture}") {
                path = $"/{culture}/";
            }
            if (path != $"/{culture}/" && path.EndsWith('/')) {
                path = path[..^1];
            }
        }

        if (path != currentPath) {
            ctx.Response.Redirect(URL.WithQuery(path, URL.Query()));
        }
    }
}
