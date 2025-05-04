namespace Jumpeno.Shared.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS8618

public class HTTP : StaticService<HTTP>, IDisposable {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static Func<HttpClient> Client;
    private static Func<int, AppException, Task> OnRefresh;
    private static Func<Exception, string?, Task> OnError;
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
        Func<int, AppException, Task> onRefresh, Func<Exception, string?, Task> onError, Action<HttpRequestMessage>? addClientCookies = null
    ) {
        if (Client is not null) return;
        Client = AppEnvironment.GetService<HttpClient>;
        OnRefresh = onRefresh;
        OnError = onError;
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
        // Retry on refresh:
        for (int iteration = 1; iteration <= 2; iteration++) {
            // Access instance:
            var instance = Instance();
            var requestID = GetRequestID(method, url);
            HttpResponseMessage? response;
            int code = CODE.DEFAULT;
            var token = new CancellationTokenSource();
            try {            
                // Add query parameters:
                if (query is not null) url = URL.SetQueryParams(url, query);

                // Create request object:
                var request = new HttpRequestMessage(method, url);

                // Add authorization:
                if (URL.IsLocal(url)) try { SetHeader(request, HEADER.AUTHORIZATION, $"{AUTH.BEARER} {Token.Access.raw}"); } catch {}

                // Add body:
                if (
                    !(new [] { HttpMethod.Head, HttpMethod.Get, HttpMethod.Options, HttpMethod.Trace }).Contains(method)
                    && body is not null
                ) {
                    var jsonBody = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, CONTENT_TYPE.JSON);
                }

                // Add headers:
                SetHeader(request, HEADER.ACCEPT_LANGUAGE, I18N.Culture);
                if (headers is not null) {
                    foreach (var header in headers) {
                        SetHeader(request, header.Key, header.Value);
                    }
                }

                // Add content headers:
                SetContentHeader(request, HEADER.CONTENT_TYPE, CONTENT_TYPE.JSON);
                if (contentHeaders is not null) {
                    foreach (var header in contentHeaders) {
                        SetContentHeader(request, header.Key, header.Value);
                    }
                }

                // Add cookies:
                if (AppEnvironment.IsServer && URL.IsLocal(url) && AddClientCookies is not null) AddClientCookies(request);

                // Store token:
                await instance.ReplaceToken(requestID, token);

                // Send request:
                response = await Client().SendAsync(request, token.Token);
                code = (int) response.StatusCode;
            } catch (OperationCanceledException) {
                throw EXCEPTION.REQUEST_CANCELLED;
            } catch {
                throw EXCEPTION.REQUEST_FAILED;
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
                    var exception = EXCEPTION.PARSING_ERROR
                    .SetHeaders(response.Headers).SetContentHeaders(response.Content.Headers);
                    throw exception;
                }
            } else {
                // Error:
                AppException exception;
                try {
                    string jsonResponse = await response!.Content.ReadAsStringAsync();
                    var json = JObject.Parse(jsonResponse);
                    // Info:
                    TInfo info;
                    try { info = json[nameof(AppException.Info)]!.ToObject<TInfo>()!; }
                    catch { info = new(MESSAGE.DEFAULT); }
                    // Errors:
                    List<Error> errors;
                    try { errors = json[nameof(AppException.Errors)]!.ToObject<List<Error>>()!; }
                    catch { errors = []; }
                    // Data:
                    IDictionary data;
                    try { data = json[nameof(AppException.Data)]!.ToObject<IDictionary>()!; }
                    catch { data = new Dictionary<object, object>(); }
                    // Code & headers:
                    exception = EXCEPTION.DEFAULT.SetCode(code)
                    .SetHeaders(response?.Headers).SetContentHeaders(response?.Content.Headers)
                    .SetInfo(info).SetErrors(errors)
                    .SetData(data);
                } catch {
                    exception = EXCEPTION.DEFAULT.SetCode(code)
                    .SetHeaders(response?.Headers).SetContentHeaders(response?.Content.Headers);
                }
                // Try to refresh token:
                if (exception.Code == EXCEPTION.NOT_AUTHENTICATED.Code) await OnRefresh(iteration, exception);
                else throw exception;
            }
        }
        throw EXCEPTION.DEFAULT;
    }

    // Tokens -----------------------------------------------------------------------------------------------------------------------------
    public static async Task ClearTokens() {
        var instance = Instance(); await instance.TokenLock.TryExclusive(() => {
            foreach (var token in instance.Tokens.Values) CancelToken(token);
            instance.Tokens.Clear();
        });
    }

    // Requests ---------------------------------------------------------------------------------------------------------------------------
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

    // Try --------------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> Try(Func<Task> callback, string? form = null) {
        try { await callback(); return true; }
        catch (AppException e) { if (e.Code != CODE.REQUEST_CANCELLED) await OnError(e, form); }
        catch (Exception e) { await OnError(e, form); }
        return false;
    }
}
