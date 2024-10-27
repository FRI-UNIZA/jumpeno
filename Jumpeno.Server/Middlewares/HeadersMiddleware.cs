namespace Jumpeno.Server.Middlewares;

public class HeadersMiddleware(RequestDelegate next) {
    private readonly RequestDelegate _next = next;

    public static void ApplyHeaders(HttpContext ctx) {
        ctx.Response.Headers.ContentLanguage = I18N.Culture();
    }

    public async Task InvokeAsync(HttpContext ctx) {
        ApplyHeaders(ctx);
        await _next(ctx);
    }
}
