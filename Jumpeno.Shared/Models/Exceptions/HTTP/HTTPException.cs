namespace Jumpeno.Shared.Exceptions;

using Newtonsoft.Json.Linq;

public class HTTPException : Exception {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int DEFAULT_CODE = 500;
    public const string DEFAULT_MESSAGE = "Something went wrong.";

    // Code -------------------------------------------------------------------------------------------------------------------------------
    public int Code { get; set; }

    // Headers ----------------------------------------------------------------------------------------------------------------------------
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

    // Message ----------------------------------------------------------------------------------------------------------------------------
    public override string Message { get; }
    public void SetMessage(string message) => Reflex.SetField(typeof(Exception), this, "_message", message);

    // Errors -----------------------------------------------------------------------------------------------------------------------------
    public List<Error> Errors { get; }

    // Data -------------------------------------------------------------------------------------------------------------------------------
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

    // Inner exception --------------------------------------------------------------------------------------------------------------------
    public void SetInner(Exception inner) => Reflex.SetField(typeof(Exception), this, "_innerException", inner);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public HTTPException(
        int code = DEFAULT_CODE,
        HttpResponseHeaders? headers = null, HttpContentHeaders? contentHeaders = null,
        string? message = null, List<Error>? errors = null, IDictionary? data = null
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
        else Message = DEFAULT_MESSAGE;

        // Set errors:
        if (errors is not null) Errors = errors;
        else Errors = [];

        // Set data:
        if (data is not null) Data = data;
        else Data = new Dictionary<object, object>();
    }

    public HTTPException(CoreException e, HttpResponseHeaders? headers = null, HttpContentHeaders? contentHeaders = null)
    : this(e.Code, headers, contentHeaders, e.Message, e.Errors, e.Data) {}
}
