namespace Jumpeno.Server.Middlewares;

public class APIMiddleware(RequestDelegate next) {
    private readonly RequestDelegate Next = next;

    public async Task InvokeAsync(HttpContext ctx) {
        if (!ctx.Request.Path.StartsWithSegments(API.BASE.PREFIX, StringComparison.OrdinalIgnoreCase)) {
            await Next(ctx); return;
        }
        // Get endpoint metadata:
        var endpoint = ctx.GetEndpoint() ?? throw EXCEPTION.BAD_REQUEST;

        // Get controller and action metadata:
        var controllerActionDescriptor = endpoint.Metadata
            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .FirstOrDefault() ?? throw EXCEPTION.BAD_REQUEST;

        // Get the method info:
        var methodInfo = controllerActionDescriptor.MethodInfo ?? throw EXCEPTION.BAD_REQUEST;
        // Move to next:
        await Next(ctx);
    }
}
