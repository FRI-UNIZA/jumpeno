namespace Jumpeno.Shared.Models;

public class HTTPResult<T> : HTTPHeadResult {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public T Body { get; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public HTTPResult(
        T body, int code = 200,
        HttpResponseHeaders? headers = null, HttpContentHeaders? contentHeaders = null
    ) : base(code, headers, contentHeaders) {
        if (body is null) throw new InvalidDataException("Data can not be empty!");
        Body = body;
    }

    public HTTPResult(
        int code,
        HttpResponseHeaders? headers, HttpContentHeaders? contentHeaders,
        T body
    ) : this(body, code, headers, contentHeaders) {}

    public HTTPResult(int code, HttpResponseHeaders? headers, T body) : this(body, code, headers, null) {}
    public HTTPResult(int code, HttpContentHeaders? contentHeaders, T body) : this(body, code, null, contentHeaders) {}
    public HTTPResult(HttpResponseHeaders? headers, HttpContentHeaders? contentHeaders, T body) : this(body, headers: headers, contentHeaders: contentHeaders) {}
    public HTTPResult(int code, T body) : this(code, (HttpResponseHeaders?) null, body) {}
    public HTTPResult(HttpResponseHeaders? headers, T body) : this(body, headers: headers) {}
    public HTTPResult(HttpContentHeaders? contentHeaders, T body) : this(body, contentHeaders: contentHeaders) {}
}
