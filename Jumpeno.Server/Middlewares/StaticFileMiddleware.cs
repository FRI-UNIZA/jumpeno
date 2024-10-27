namespace Jumpeno.Server.Middlewares;

public class StaticFileMiddleware(RequestDelegate next) {
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext ctx) {
        await ServerEnvironment.WithoutStaticPath(ctx.Request.Path, async () => {
            await _next(ctx);
        });
    }
}
