namespace Jumpeno.Server.Middlewares;

public class DisposeMiddleware(RequestDelegate next) {
    public async Task InvokeAsync(HttpContext ctx) {
        await next(ctx);
        await Disposer.RequestDispose();
    }
}
