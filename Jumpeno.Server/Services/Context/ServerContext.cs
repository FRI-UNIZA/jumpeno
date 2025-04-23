namespace Jumpeno.Server.Services;

public static class ServerContext {
    public static HttpContext Instance => AppEnvironment.GetService<IHttpContextAccessor>().HttpContext!;

    public static void Respond(HTTPHeadResult result) {
        var ctx = Instance;
        ctx.Response.StatusCode = result.Code;
        foreach (var header in result.Headers) {
            ctx.Response.Headers[header.Key] = header.Value.FirstOrDefault();
        }
        foreach (var header in result.ContentHeaders) {
            ctx.Response.Headers[header.Key] = header.Value.FirstOrDefault();
        }
    }

    public static T Respond<T>(HTTPResult<T> result) {
        Respond((HTTPHeadResult) result);
        return result.Body;
    }
}
