namespace Jumpeno.Server.Middlewares;

public class StaticFileMiddleware(RequestDelegate next) {
    private readonly RequestDelegate Next = next;

    public async Task InvokeAsync(HttpContext ctx) {
        await ServerEnvironment.WithoutStaticPath(ctx.Request.Path, async () => await Next(ctx));
    }
}
