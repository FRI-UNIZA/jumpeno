namespace Jumpeno.Server.Middlewares;

using Newtonsoft.Json;

public class ErrorMiddleware(RequestDelegate next) {
    private readonly RequestDelegate Next = next;

    public async Task InvokeAsync(HttpContext ctx) {
        Exception? exception = null;

        try {
            await Next(ctx);
        } catch (HTTPException e) {
            ctx.Response.StatusCode = e.Code;
            foreach (var header in e.Headers) {
                ctx.Response.Headers[header.Key] = header.Value.FirstOrDefault();
            }
            foreach (var header in e.ContentHeaders) {
                ctx.Response.Headers[header.Key] = header.Value.FirstOrDefault();
            }
            exception = e;
        } catch (CoreException e) {
            ctx.Response.StatusCode = e.Code;
            exception = e;
        } catch {
            ctx.Response.StatusCode = 500;
            exception = new CoreException();
        }

        if (exception is not null) {
            ctx.Response.Headers.ContentType = CONTENT_TYPE.JSON;
            var errors = exception switch {
                HTTPException e => e.Errors,
                CoreException e => e.Errors,
                _ => []
            };
            foreach (var error in errors) {
                error.Message = I18N.T(error.Message, error.Values, unsplit: true);
            }
            await ctx.Response.WriteAsync(JsonConvert.SerializeObject(new {
                Message = I18N.T(exception.Message, unsplit: true),
                Errors = errors,
                exception.Data
            }));
        }
    }
}
