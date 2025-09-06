namespace Jumpeno.Server.Middlewares;

public class AuthMiddleware(RequestDelegate next) {
    private readonly RequestDelegate Next = next;

    public async Task InvokeAsync(HttpContext ctx) {
        JWT.Authorize(ctx);
        await Next(ctx);
    }
}
