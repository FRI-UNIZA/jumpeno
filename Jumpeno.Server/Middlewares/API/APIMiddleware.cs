namespace Jumpeno.Server.Middlewares;

public class APIMiddleware(RequestDelegate next) {
    public async Task InvokeAsync(HttpContext ctx) {
        if (!ctx.Request.Path.StartsWithSegments(API.BASE.PREFIX, StringComparison.OrdinalIgnoreCase)) {
            await next(ctx); return;
        }
        // Get endpoint metadata:
        var endpoint = ctx.GetEndpoint() ?? throw Exceptions.BAD_REQUEST;

        // Get controller and action metadata:
        var controllerActionDescriptor = endpoint.Metadata
            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .FirstOrDefault() ?? throw Exceptions.BAD_REQUEST;

        // Check the method info:
        if (controllerActionDescriptor?.MethodInfo == null) throw Exceptions.BAD_REQUEST;
        // Move to next:
        await next(ctx);
    }
}
