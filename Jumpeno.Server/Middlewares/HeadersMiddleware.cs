namespace Jumpeno.Server.Middlewares;

public class HeadersMiddleware(RequestDelegate next) {
    private readonly RequestDelegate Next = next;

    public static void ApplyHeaders(HttpContext ctx) {
        ctx.Response.Headers.ContentLanguage = I18N.Culture;
    }

    public async Task InvokeAsync(HttpContext ctx) {
        ApplyHeaders(ctx);
        await Next(ctx);
    }
}
