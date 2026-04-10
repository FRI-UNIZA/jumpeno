namespace Jumpeno.Server.Middlewares;

public class VersionMiddleware(RequestDelegate next) {
    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private static void CheckVersion(string? version) {
        if (version == AppSettings.Version) return;
        throw Exceptions.CLIENT.SetInfo("Incorrect version! Please refresh.");
    }

    public static void CheckHubVersion(HttpContext ctx) {
        if (ctx.Request.Query.TryGetValue(Header.APP_VERSION, out var version)) CheckVersion(version);
    }

    public static void CheckApiVersion(HttpContext ctx) {
        if (ctx.Request.Headers.TryGetValue(Header.APP_VERSION, out var version)) CheckVersion(version);
    }

    // Invoke -----------------------------------------------------------------------------------------------------------------------------
    public async Task InvokeAsync(HttpContext ctx) {
        CheckApiVersion(ctx);
        await next(ctx);
    }
}
