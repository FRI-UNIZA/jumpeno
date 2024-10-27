namespace Jumpeno.Shared.Models;

public class HTTPException: Exception {
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
    public override string Message { get; }
    public override IDictionary Data { get; }
    private T? GetData<T>(string key) {
        try { return (T) Data[key]!; }
        catch { return default; }
    }
    public string? GetString(string key) {
        return GetData<string>(key);
    }
    public JObject? GetObject(string key) {
        return GetData<JObject>(key);
    }
    public JArray? GetArray(string key) {
        return GetData<JArray>(key);
    }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public HTTPException(
        int code = 500,
        HttpResponseHeaders? headers = null, HttpContentHeaders? contentHeaders = null,
        string? message = null, IDictionary? data = null
    ) {
        // Set code:
        Code = code;

        // Set headers:
        if (headers is not null) Headers = headers;
        else Headers = HTTPHeadResult.EmptyHeaders();
        if (contentHeaders is not null) ContentHeaders = contentHeaders;
        else ContentHeaders = HTTPHeadResult.EmptyContentHeaders();

        // Set message:
        if (message is not null) Message = message;
        else Message = I18N.T("Something went wrong.");

        // Set data:
        if (data is not null) Data = data;
        else Data = new Dictionary<object, object>();
    }
}
