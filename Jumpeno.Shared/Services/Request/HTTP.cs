namespace Jumpeno.Shared.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS8618
#pragma warning disable CA1816

public class HTTP : StaticService<HTTP>, IDisposable {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int STATUS_FAILED = 600;
    public const int STATUS_CANCELLED = 601;
    public const int STATUS_PARSING_ERROR = 602;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static Func<HttpClient> Client;
    private static Func<Exception, Task> OnError;
    private static Func<string?> AccessToken;
    private static Action<HttpRequestMessage>? AddClientCookies;
    private readonly Dictionary<string, CancellationTokenSource> Tokens = [];
    private readonly LockerSlim TokenLock = new();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public HTTP() => Disposer = new(this, DisposeUnmanaged);
    private readonly Disposer Disposer;
    private void DisposeUnmanaged() {
        foreach (var token in Tokens.Values) token.Dispose();
        TokenLock.Dispose();
    }
    public void Dispose() => Disposer.Dispose();
    ~HTTP() => Disposer.Final();

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(
        Func<Exception, Task> onError, Func<string?> accessToken, Action<HttpRequestMessage>? addClientCookies = null
    ) {
        if (Client is not null) return;
        Client = AppEnvironment.GetService<HttpClient>;
        OnError = onError;
        AccessToken = accessToken;
        AddClientCookies = addClientCookies;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static void SetHeader(HttpRequestMessage request, string key, string value) {
        if (request.Headers.Contains(key)) {
            request.Headers.Remove(key);
        }
        request.Headers.Add(key, value);
    }
    
    private static void SetContentHeader(HttpRequestMessage request, string key, string value) {
        if (request.Content is null) return;
        if (request.Content.Headers.Contains(key)) {
            request.Content.Headers.Remove(key);
        }
        request.Content.Headers.Add(key, value);
    }

    private static string GetRequestID(HttpMethod method, string url) {
        return $"{method}:{url}";
    }

    private static void CancelToken(CancellationTokenSource token) {
        token.Cancel();
        token.Dispose();
    }

    private async Task RemoveToken(string requestID, CancellationTokenSource token) {
        await TokenLock.TryExclusive(() => {
            if (!Tokens.TryGetValue(requestID, out var stored)) return;
            if (stored != token) return;
            CancelToken(token);
            Tokens.Remove(requestID);
        });
    }

    private async Task ReplaceToken(string requestID, CancellationTokenSource token) {
        await TokenLock.TryExclusive(() => {
            if (Tokens.TryGetValue(requestID, out var stored)) CancelToken(stored);
            Tokens[requestID] = token;
        });
    }

    private static async Task<HTTPHeadResult> Request<T>(
        HttpMethod method, bool bodyAccess, string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        // Access instance:
        var instance = Instance();
        var requestID = GetRequestID(method, url);
        HttpResponseMessage? response;
        int code;
        var token = new CancellationTokenSource(); // NOTE: Cancel token
        try {            
            // Add query parameters:
            if (query is not null) {
                url = URL.SetQueryParams(url, query);
            }

            // Create request object:
            var request = new HttpRequestMessage(method, url);

            // Add authorization:
            if (URL.IsLocal(url)) {
                var accessToken = AccessToken();
                if (accessToken != null) SetHeader(request, "Authorization", $"Bearer {accessToken}");
            }

            // Add body:
            if (
                !(new [] { HttpMethod.Head, HttpMethod.Get, HttpMethod.Options, HttpMethod.Trace }).Contains(method)
                && body is not null
            ) {
                var jsonBody = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            }

            // Add headers:
            SetHeader(request, "Accept-Language", I18N.Culture);
            if (headers is not null) {
                foreach (var header in headers) {
                    SetHeader(request, header.Key, header.Value);
                }
            }

            // Add content headers:
            SetContentHeader(request, "Content-Type", CONTENT_TYPE.JSON);
            if (contentHeaders is not null) {
                foreach (var header in contentHeaders) {
                    SetContentHeader(request, header.Key, header.Value);
                }
            }

            // Add cookies:
            if (AppEnvironment.IsServer && URL.IsLocal(url) && AddClientCookies is not null) {
                AddClientCookies(request);
            }

            // Store token:
            await instance.ReplaceToken(requestID, token);

            // Send request:
            response = await Client().SendAsync(request, token.Token);
            code = (int) response.StatusCode;
        } catch (OperationCanceledException) {
            throw new HTTPException(code: STATUS_CANCELLED, message: "Request cancelled.");
        } catch {
            var exception = new HTTPException(code: STATUS_FAILED, message: "Request failed.");
            throw exception;
        } finally {
            await instance.RemoveToken(requestID, token);
        }

        // Check status code:
        if (response.IsSuccessStatusCode && response is not null) {
            // Success - convert http response data to object:
            try {
                if (bodyAccess) return new HTTPResult<T>(code, response.Headers, response.Content.Headers, (await response.Content.ReadFromJsonAsync<T>())!);
                return new HTTPHeadResult(code, response.Headers, response.Content.Headers);
            } catch {
                var exception = new HTTPException(STATUS_PARSING_ERROR, response.Headers, response.Content.Headers, "Parsing error.");
                throw exception;
            }
        } else {
            // Error:
            HTTPException exception;
            try {
                string jsonResponse = await response!.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonResponse);
                string message;
                try { message = json["Message"]!.ToString(); }
                catch { message = ""; }
                if (message is null || message == "") {
                    message = "Something went wrong.";
                }
                List<Error> errors;
                try { errors = json["Errors"]!.ToObject<List<Error>>()!; }
                catch { errors = []; }
                var data = json["Data"]!.ToObject<IDictionary>();
                exception = new HTTPException(code, response?.Headers, response?.Content.Headers, message, errors, data);
            } catch {
                exception = new HTTPException(code, response?.Headers, response?.Content.Headers, "Something went wrong.");
            }
            throw exception;
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task ClearTokens() {
        var instance = Instance(); await instance.TokenLock.TryExclusive(() => {
            foreach (var token in instance.Tokens.Values) CancelToken(token);
            instance.Tokens.Clear();
        });
    }

    public static async Task<HTTPHeadResult> Head(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null
    ) {
        return await Request<object>(HttpMethod.Head, false, url, query, headers);
    }

    public static async Task<HTTPHeadResult> Options(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null
    ) {
        return await Request<object>(HttpMethod.Options, false, url, query, headers);
    }
    public static async Task<HTTPResult<T>> Options<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Options, true, url, query, headers);
    }

    public static async Task<HTTPHeadResult> Trace(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null
    ) {
        return await Request<object>(HttpMethod.Trace, false, url, query, headers);
    }
    public static async Task<HTTPResult<T>> Trace<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Trace, true, url, query, headers);
    }

    public static async Task<HTTPHeadResult> Get(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null
    ) {
        return await Request<object>(HttpMethod.Get, false, url, query, headers);
    }
    public static async Task<HTTPResult<T>> Get<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Get, true, url, query, headers);
    }

    public static async Task<HTTPHeadResult> Connect(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return await Request<object>(HttpMethod.Connect, false, url, query, headers, contentHeaders, body);
    }
    public static async Task<HTTPResult<T>> Connect<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Connect, true, url, query, headers, contentHeaders, body);
    }

    public static async Task<HTTPHeadResult> Post(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return await Request<object>(HttpMethod.Post, false, url, query, headers, contentHeaders, body);
    }
    public static async Task<HTTPResult<T>> Post<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Post, true, url, query, headers, contentHeaders, body);
    }

    public static async Task<HTTPHeadResult> Put(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return await Request<object>(HttpMethod.Put, false, url, query, headers, contentHeaders, body);
    }
    public static async Task<HTTPResult<T>> Put<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Put, true, url, query, headers, contentHeaders, body);
    }

    public static async Task<HTTPHeadResult> Patch(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return await Request<object>(HttpMethod.Patch, false, url, query, headers, contentHeaders, body);
    }
    public static async Task<HTTPResult<T>> Patch<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Patch, true, url, query, headers, contentHeaders, body);
    }

    public static async Task<HTTPHeadResult> Delete(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return await Request<object>(HttpMethod.Delete, false, url, query, headers, contentHeaders, body);
    }
    public static async Task<HTTPResult<T>> Delete<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Delete, true, url, query, headers, contentHeaders, body);
    }

    public static async Task<byte[]> Download(string url) {
        // return await Client().GetByteArrayAsync(url);

        var response = await Client().GetAsync(url);

        if (response.IsSuccessStatusCode) {
            return await response.Content.ReadAsByteArrayAsync();
        } else {
            throw new Exception();
        }
    }

    public static async Task Try(Func<Task> callback) {
        try { await callback(); }
        catch (Exception e) { await OnError(e); }
    }
}
