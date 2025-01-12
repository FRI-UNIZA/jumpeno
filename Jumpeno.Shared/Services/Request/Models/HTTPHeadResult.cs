namespace Jumpeno.Shared.Models;

public class HTTPHeadResult {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int Code { get; }
    public HttpResponseHeaders Headers { get; }
    public string? Header(string key) {
        if (Headers.TryGetValues(key, out var headerValues)) {
            try { return headerValues.FirstOrDefault(); }
            catch { return null; }
        } else return null;
    }
    public HttpContentHeaders ContentHeaders { get; }
    public string? ContentHeader(string key) {
        if (ContentHeaders.TryGetValues(key, out var headerValues)) {
            try { return headerValues.FirstOrDefault(); }
            catch { return null; }
        } else return null;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public HTTPHeadResult(
        int code = 200,
        HttpResponseHeaders? headers = null, HttpContentHeaders? contentHeaders = null
    ) {
        // Set code:
        Code = code;

        // Set headers:
        var msg = EmptyMessage();
        if (headers is not null) Headers = headers;
        else Headers = EmptyHeaders();
        if (contentHeaders is not null) ContentHeaders = contentHeaders;
        else ContentHeaders = EmptyContentHeaders();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static HttpResponseMessage EmptyMessage() {
        var msg = new HttpResponseMessage {
            Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
        };
        msg.Headers.Clear();
        msg.Content.Headers.Clear();
        return msg;
    }
    public static HttpResponseHeaders EmptyHeaders() { return EmptyMessage().Headers; }
    public static HttpContentHeaders EmptyContentHeaders() { return EmptyMessage().Content.Headers; }
}
