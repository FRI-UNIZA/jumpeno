namespace Jumpeno.Server.Middlewares;

public class APIMiddleware(RequestDelegate next) {
    public async Task InvokeAsync(HttpContext ctx) {
        if (!ctx.Request.Path.StartsWithSegments(API.Base.Prefix, StringComparison.OrdinalIgnoreCase)) {
            await next(ctx); return;
        }
        // Get endpoint metadata:
        var endpoint = ctx.GetEndpoint() ?? throw Exceptions.BadRequest;

        // Get controller and action metadata:
        var controllerActionDescriptor = endpoint.Metadata
            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .FirstOrDefault() ?? throw Exceptions.BadRequest;

        // Check the method info:
        if (controllerActionDescriptor?.MethodInfo == null) throw Exceptions.BadRequest;
        // Move to next:
        await next(ctx);
    }
}
