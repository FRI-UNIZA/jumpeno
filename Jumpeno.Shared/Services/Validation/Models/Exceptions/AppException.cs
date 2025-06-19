namespace Jumpeno.Shared.Models;

using Newtonsoft.Json.Linq;

public class AppException : Exception {
    // Code -------------------------------------------------------------------------------------------------------------------------------
    public int Code { get; private set; }
    public AppException SetCode(int code) { Code = code; return this; }

    // Headers ----------------------------------------------------------------------------------------------------------------------------
    public HttpResponseHeaders Headers { get; private set; }
    public AppException SetHeaders(HttpResponseHeaders? headers) {
        Headers = headers ?? HTTPHeadResult.EmptyHeaders(); return this;
    }

    public HttpContentHeaders ContentHeaders { get; private set; }
    public AppException SetContentHeaders(HttpContentHeaders? contentHeaders) {
        ContentHeaders = contentHeaders ?? HTTPHeadResult.EmptyContentHeaders(); return this;
    }

    // Message ----------------------------------------------------------------------------------------------------------------------------    
    public TInfo Info { get; private set; }
    public AppException SetInfo(string key, Dictionary<string, object>? values = null) {
        Info = new(key, values);
        Reflex.SetField(typeof(Exception), this, "_message", Info.T);
        return this;
    }
    public AppException SetInfo(TInfo info) {
        Info = new(info);
        Reflex.SetField(typeof(Exception), this, "_message", info.T);
        return this;
    }

    // Errors -----------------------------------------------------------------------------------------------------------------------------
    public List<Error> Errors { get; private set; }
    public AppException SetErrors(List<Error> errors) { Errors = errors; return this; }
    public AppException SetErrors(Error error) => SetErrors([error]);
    public virtual AppException Add(Error error) => Add([error]);
    public virtual AppException Add(List<Error> errors) { Errors.AddRange(errors); return this; }
    public bool HasErrors => Errors.Count > 0;

    // Data -------------------------------------------------------------------------------------------------------------------------------
    private T? GetData<T>(string key) {
        try { return (T) Data[key]!; }
        catch { return default; }
    }
    public string? GetString(string key) => GetData<string>(key);
    public JObject? GetObject(string key) => GetData<JObject>(key);
    public JArray? GetArray(string key) => GetData<JArray>(key);
    public AppException SetData(IDictionary? data) {
        var exceptionData = Data;
        if (data != null) {
            foreach (DictionaryEntry entry in data) {
                exceptionData[entry.Key] = entry.Value;
            }
        }
        return this;
    }

    // Inner exception --------------------------------------------------------------------------------------------------------------------
    public void SetInner(Exception inner) => Reflex.SetField(typeof(Exception), this, "_innerException", inner);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor][Newtonsoft.Json.JsonConstructor]
    protected AppException(
        int code,
        HttpResponseHeaders headers, HttpContentHeaders contentHeaders,
        TInfo info, List<Error> errors,
        IDictionary data
    ) : base(info.T) {
        Code = code;
        Headers = headers;
        ContentHeaders = contentHeaders;
        Info = new(info);
        Errors = errors;
        SetData(data);
    }

    public AppException() : this(
        CODE.DEFAULT,
        HTTPHeadResult.EmptyHeaders(), HTTPHeadResult.EmptyContentHeaders(),
        new(MESSAGE.DEFAULT), [],
        new Dictionary<object, object>()
    ) {}

    // Data Transfer Object ---------------------------------------------------------------------------------------------------------------
    [JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public virtual AppExceptionDTO DTO => new(this);
}
