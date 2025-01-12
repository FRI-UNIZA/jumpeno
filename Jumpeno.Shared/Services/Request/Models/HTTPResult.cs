namespace Jumpeno.Shared.Models;

public class HTTPResult<T> : HTTPHeadResult {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public T Data { get; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public HTTPResult(
        T data, int code = 200,
        HttpResponseHeaders? headers = null, HttpContentHeaders? contentHeaders = null
    ) : base(code, headers, contentHeaders) {
        if (data is null) throw new InvalidDataException("Data can not be empty!");
        Data = data;
    }

    public HTTPResult(
        int code,
        HttpResponseHeaders? headers, HttpContentHeaders? contentHeaders,
        T data
    ) : this(data, code, headers, contentHeaders) {}

    public HTTPResult(int code, HttpResponseHeaders? headers, T data) : this(data, code, headers, null) {}
    public HTTPResult(int code, HttpContentHeaders? contentHeaders, T data) : this(data, code, null, contentHeaders) {}
    public HTTPResult(HttpResponseHeaders? headers, HttpContentHeaders? contentHeaders, T data) : this(data, headers: headers, contentHeaders: contentHeaders) {}
    public HTTPResult(int code, T data) : this(code, (HttpResponseHeaders?) null, data) {}
    public HTTPResult(HttpResponseHeaders? headers, T data) : this(data, headers: headers) {}
    public HTTPResult(HttpContentHeaders? contentHeaders, T data) : this(data, contentHeaders: contentHeaders) {}
}
