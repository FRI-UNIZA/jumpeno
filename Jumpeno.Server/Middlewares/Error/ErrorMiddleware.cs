namespace Jumpeno.Server.Middlewares;

using Newtonsoft.Json;

public class ErrorMiddleware(RequestDelegate next) {
    public async Task InvokeAsync(HttpContext ctx) {
        Exception? exception = null;
        TInfo? info = null;

        try {
            await next(ctx);
        } catch (AppException e) {
            ctx.Response.StatusCode = e.Code;
            foreach (var header in e.Headers) {
                ctx.Response.Headers[header.Key] = header.Value.FirstOrDefault();
            }
            foreach (var header in e.ContentHeaders) {
                ctx.Response.Headers[header.Key] = header.Value.FirstOrDefault();
            }
            exception = e;
            info = e.Info;
        } catch {
            ctx.Response.StatusCode = Codes.DEFAULT;
            exception = Exceptions.DEFAULT;
            info = Messages.DEFAULT;
        }

        if (exception is not null) {
            ctx.Response.Headers.ContentType = ContentType.JSON;
            var errors = exception switch {
                AppException e => e.Errors,
                _ => []
            };
            await ctx.Response.WriteAsync(JsonConvert.SerializeObject(new {
                Info = info,
                Errors = errors,
                exception.Data
            }));
        }
    }
}
