namespace Jumpeno.Server.Middlewares;

public class AuthMiddleware(RequestDelegate next) {
    public async Task InvokeAsync(HttpContext ctx) {
        JWT.Authorize(ctx);
        await next(ctx);
    }
}
