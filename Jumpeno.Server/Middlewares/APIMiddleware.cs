namespace Jumpeno.Server.Middlewares;

public class APIMiddleware(RequestDelegate next) {
    private readonly RequestDelegate Next = next;

    public async Task InvokeAsync(HttpContext ctx) {
        if (!ctx.Request.Path.StartsWithSegments(AppSettings.Api.Base.Prefix, StringComparison.OrdinalIgnoreCase)) {
            await Next(ctx); return;
        }
        // Get endpoint metadata:
        var endpoint = ctx.GetEndpoint() ?? throw Exceptions.BadRequest;

        // Get controller and action metadata:
        var controllerActionDescriptor = endpoint.Metadata
            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .FirstOrDefault() ?? throw Exceptions.BadRequest;

        // Get the method info:
        var methodInfo = controllerActionDescriptor.MethodInfo ?? throw Exceptions.BadRequest;
        // Move to next:
        await Next(ctx);
    }
}
