namespace Jumpeno.Server.Middlewares;

public class DisposeMiddleware(RequestDelegate next) {
    private readonly RequestDelegate Next = next;

    public async Task InvokeAsync(HttpContext ctx) {
        await Next(ctx);
        await Disposer.RequestDispose();
    }
}
