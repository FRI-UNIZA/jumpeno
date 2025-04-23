namespace Jumpeno.Server.Middlewares;

public class HeadersMiddleware(RequestDelegate next) {
    private readonly RequestDelegate Next = next;

    private static void ApplyHeaders(HttpContext ctx) {
        // 1) Culture header:
        ctx.Response.Headers.ContentLanguage = I18N.Culture;        
        // 2) Content header:
        var endpoint = ctx.GetEndpoint();
        if (endpoint == null) return;
        var controllerActionDescriptor = endpoint.Metadata
            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .FirstOrDefault();
        if (controllerActionDescriptor == null) return;
        ctx.Response.Headers.ContentType = CONTENT_TYPE.JSON;
    }

    public async Task InvokeAsync(HttpContext ctx) {
        ApplyHeaders(ctx);
        await Next(ctx);
    }
}
