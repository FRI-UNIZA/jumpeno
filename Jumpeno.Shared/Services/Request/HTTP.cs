namespace Jumpeno.Shared.Services;

using Newtonsoft.Json;

#pragma warning disable CS8618

public class HTTP: StaticService<HTTP> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int STATUS_FAILED = 600;
    public const int STATUS_CANCELLED = 601;
    public const int STATUS_PARSING_ERROR = 602;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static Func<HttpClient> Client;
    private static Func<HTTPException, Task> OnError;
    private static Action<HttpRequestMessage>? AddClientCookies;
    private readonly Dictionary<string, CancellationTokenSource> Tokens = [];
    private readonly SemaphoreSlim TokenLock = new(1, 1);

    // Initializers -----------------------------------------------------------------------------------------------------------------------
    public static void Init(Func<HTTPException, Task> onError, Action<HttpRequestMessage>? addClientCookies = null) {
        if (Client is not null) return;
        Client = AppEnvironment.GetService<HttpClient>;
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

    private async Task RemoveToken(string requestID) {
        await TokenLock.WaitAsync();
        Tokens.Remove(requestID);
        TokenLock.Release();
    }

    private static async Task<HTTPHeadResult> Request<T>(
        HttpMethod method, bool bodyAccess, string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        // Access instance:
        var instance = Instance();
        var requestID = GetRequestID(method, url);
        HttpResponseMessage? response;
        int code;
        try {
            // TODO: Add authorization for base url
            
            // Add query parameters:
            if (query is not null) {
                url = URL.SetQueryParams(url, query);
            }

            // Create request object:
            var request = new HttpRequestMessage(method, url);

            // Add body:
            if (
                !(new [] { HttpMethod.Head, HttpMethod.Get, HttpMethod.Options, HttpMethod.Trace }).Contains(method)
                && body is not null
            ) {
                var jsonBody = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            }

            // Add headers:
            SetHeader(request, "Accept-Language", I18N.Culture());
            if (headers is not null) {
                foreach (var header in headers) {
                    SetHeader(request, header.Key, header.Value);
                }
            }

            // Add content headers:
            SetContentHeader(request, "Content-Type", CONTENT_TYPE.JSON_UTF8);
            if (contentHeaders is not null) {
                foreach (var header in contentHeaders) {
                    SetContentHeader(request, header.Key, header.Value);
                }
            }

            // Add cookies:
            if (AppEnvironment.IsServer && URL.IsLocal(url) && AddClientCookies is not null) {
                AddClientCookies(request);
            }

            // Cancel token:
            await instance.TokenLock.WaitAsync();
            instance.Tokens.TryGetValue(requestID, out var previousToken);
            previousToken?.Cancel();
            var token = new CancellationTokenSource();
            instance.Tokens[requestID] = token;
            instance.TokenLock.Release();

            // Send request:
            response = await Client().SendAsync(request, token.Token);
            code = (int) response.StatusCode;
            await instance.RemoveToken(requestID);
        } catch (OperationCanceledException) {
            throw new HTTPException(code: STATUS_CANCELLED, message: I18N.T("Request cancelled."));
        } catch {
            var exception = new HTTPException(code: STATUS_FAILED, message: I18N.T("Request failed."));
            if (handleError) await OnError(exception);
            await instance.RemoveToken(requestID);
            throw exception;
        }

        // Check status code:
        if (response.IsSuccessStatusCode && response is not null) {
            // Success - convert http response data to object:
            try {
                if (bodyAccess) return new HTTPResult<T>(code, response.Headers, response.Content.Headers, (await response.Content.ReadFromJsonAsync<T>())!);
                return new HTTPHeadResult(code, response.Headers, response.Content.Headers);
            } catch {
                var exception = new HTTPException(STATUS_PARSING_ERROR, response.Headers, response.Content.Headers, I18N.T("Parsing error."));
                if (handleError) await OnError(exception);
                throw exception;
            }
        } else {
            // Error:
            HTTPException exception;
            try {
                string jsonResponse = await response!.Content.ReadAsStringAsync();
                Dictionary<object, object> data = JsonConvert.DeserializeObject<Dictionary<object, object>>(jsonResponse)!;
                string message;
                try { message = (data["message"] as string)!; }
                catch { message = ""; }
                if (message is null || message == "") {
                    message = I18N.T("Something went wrong.");
                }
                exception = new HTTPException(code, response?.Headers, response?.Content.Headers, message, data);
            } catch {
                exception = new HTTPException(code, response?.Headers, response?.Content.Headers, I18N.T("Something went wrong."));
            }
            if (handleError) await OnError(exception);
            throw exception;
        }
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task ClearTokens() {
        var instance = Instance();
        await instance.TokenLock.WaitAsync();
        foreach (var token in instance.Tokens) {
            token.Value.Cancel();
        }
        instance.Tokens.Clear();
        instance.TokenLock.Release();
    }

    public static async Task<HTTPHeadResult> Head(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Head, false, url, query, headers, handleError: handleError);
    }

    public static async Task<HTTPHeadResult> Options(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Options, false, url, query, headers, handleError: handleError);
    }
    public static async Task<HTTPResult<T>> Options<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        bool handleError = true
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Options, true, url, query, headers, handleError: handleError);
    }

    public static async Task<HTTPHeadResult> Trace(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Trace, false, url, query, headers, handleError: handleError);
    }
    public static async Task<HTTPResult<T>> Trace<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        bool handleError = true
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Trace, true, url, query, headers, handleError: handleError);
    }

    public static async Task<HTTPHeadResult> Get(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Get, false, url, query, headers, handleError: handleError);
    }
    public static async Task<HTTPResult<T>> Get<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        bool handleError = true
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Get, true, url, query, headers, handleError: handleError);
    }

    public static async Task<HTTPHeadResult> Connect(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Connect, false, url, query, headers, contentHeaders, body, handleError);
    }
    public static async Task<HTTPResult<T>> Connect<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Connect, true, url, query, headers, contentHeaders, body, handleError);
    }

    public static async Task<HTTPHeadResult> Post(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Post, false, url, query, headers, contentHeaders, body, handleError);
    }
    public static async Task<HTTPResult<T>> Post<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Post, true, url, query, headers, contentHeaders, body, handleError);
    }

    public static async Task<HTTPHeadResult> Put(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Put, false, url, query, headers, contentHeaders, body, handleError);
    }
    public static async Task<HTTPResult<T>> Put<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Put, true, url, query, headers, contentHeaders, body, handleError);
    }

    public static async Task<HTTPHeadResult> Patch(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Patch, false, url, query, headers, contentHeaders, body, handleError);
    }
    public static async Task<HTTPResult<T>> Patch<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Patch, true, url, query, headers, contentHeaders, body, handleError);
    }

    public static async Task<HTTPHeadResult> Delete(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return await Request<object>(HttpMethod.Delete, false, url, query, headers, contentHeaders, body, handleError);
    }
    public static async Task<HTTPResult<T>> Delete<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        bool handleError = true
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Delete, true, url, query, headers, contentHeaders, body, handleError);
    }
}
