namespace Jumpeno.Server.Middlewares;

using Newtonsoft.Json;

public class ErrorMiddleware(RequestDelegate next) {
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext ctx) {
        Exception? exception = null;
    
        try {
            await _next(ctx);
        } catch (HTTPException e) {
            ctx.Response.StatusCode = e.Code;
            foreach (var header in e.Headers) {
                ctx.Response.Headers[header.Key] = header.Value.FirstOrDefault();
            }
            foreach (var header in e.ContentHeaders) {
                ctx.Response.Headers[header.Key] = header.Value.FirstOrDefault();
            }
            exception = e;
        } catch (Exception e) {
            ctx.Response.StatusCode = 500;
            exception = e;
        }

        if (exception is not null) {
            ctx.Response.Headers.ContentType = CONTENT_TYPE.JSON_UTF8;
            await ctx.Response.WriteAsync(JsonConvert.SerializeObject(new {
                exception.Message,
                Errors = exception is HTTPException e ? e.Errors : [],
                exception.Data
            }));
        }
    }
}
