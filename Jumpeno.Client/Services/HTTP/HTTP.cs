namespace Jumpeno.Client.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS8618

public class HTTP {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static Func<HttpClient> Client;
    private static Func<int, AppException, Task> OnRefresh;
    private static Func<Exception, string?, Task> OnError;
    private static Func<EmptyResponse<bool>, Task<bool>> TabLock;
    private static Action<HttpRequestMessage>? AddClientCookies;

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(
        Func<int, AppException, Task> onRefresh,
        Func<Exception, string?, Task> onError,
        Func<EmptyResponse<bool>, Task<bool>> tabLock,
        Action<HttpRequestMessage>? addClientCookies = null
    ) {
        InitOnce.Check(nameof(HTTP));
        Client = AppEnvironment.GetService<HttpClient>;
        OnRefresh = onRefresh;
        OnError = onError;
        TabLock = tabLock;
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

    private static async Task<HTTPHeadResult> Request<T>(
        HttpMethod method, bool bodyAccess, string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        // Retry on refresh:
        for (int iteration = 1; iteration <= 2; iteration++) {
            // Access instance:
            HttpResponseMessage? response;
            int code = Codes.Default;
            bool isLocalURL = URL.IsLocal(url);
            // Cancel token:
            var cts = new CancellationTokenSource();
            if (token != null) {
                token.Token = cts;
                if (token.IsCancelled) token.Cancel();
            }
            try {            
                // Add query parameters:
                if (query is not null) url = URL.SetQueryParams(url, query);

                // Create request object:
                var request = new HttpRequestMessage(method, url);

                if (isLocalURL) {
                    // Add authorization:
                    try { SetHeader(request, Header.Authorization, $"{AuthTypes.Bearer} {Token.Access.raw}"); } catch {}
                    // Add version:
                    SetHeader(request, Header.AppVersion, AppSettings.Version);
                }

                // Add body:
                if (
                    !(new [] { HttpMethod.Head, HttpMethod.Get, HttpMethod.Options, HttpMethod.Trace }).Contains(method)
                    && body is not null
                ) {
                    var jsonBody = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, ContentType.Json);
                }

                // Add headers:
                SetHeader(request, Header.AcceptLanguage, I18N.Culture);
                if (headers is not null) {
                    foreach (var header in headers) {
                        SetHeader(request, header.Key, header.Value);
                    }
                }

                // Add content headers:
                SetContentHeader(request, Header.ContentType, ContentType.Json);
                if (contentHeaders is not null) {
                    foreach (var header in contentHeaders) {
                        SetContentHeader(request, header.Key, header.Value);
                    }
                }

                // Add cookies:
                if (AppEnvironment.IsServer && isLocalURL && AddClientCookies is not null) AddClientCookies(request);

                // Send request:
                response = await Client().SendAsync(request, cts.Token);
                code = (int) response.StatusCode;
            } catch (OperationCanceledException) {
                throw Exceptions.RequestCancelled;
            } catch {
                throw Exceptions.RequestFailed;
            } finally {
                cts.Dispose();
            }

            // Check status code:
            if (response.IsSuccessStatusCode && response is not null) {
                // Success - convert http response data to object:
                try {
                    if (bodyAccess) return new HTTPResult<T>(code, response.Headers, response.Content.Headers, (await response.Content.ReadFromJsonAsync<T>())!);
                    return new HTTPHeadResult(code, response.Headers, response.Content.Headers);
                } catch {
                    var exception = Exceptions.ParsingError
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
                    catch { info = new(Messages.Default); }
                    // Errors:
                    List<Error> errors;
                    try { errors = json[nameof(AppException.Errors)]!.ToObject<List<Error>>()!; }
                    catch { errors = []; }
                    // Data:
                    IDictionary data;
                    try { data = json[nameof(AppException.Data)]!.ToObject<IDictionary>()!; }
                    catch { data = new Dictionary<object, object>(); }
                    // Code & headers:
                    exception = Exceptions.Default.SetCode(code)
                    .SetHeaders(response?.Headers).SetContentHeaders(response?.Content.Headers)
                    .SetInfo(info).SetErrors(errors)
                    .SetData(data);
                } catch {
                    exception = Exceptions.Default.SetCode(code)
                    .SetHeaders(response?.Headers).SetContentHeaders(response?.Content.Headers);
                }
                // Try to refresh token:
                if (isLocalURL && exception.Code == Exceptions.NotAuthenticated.Code) await OnRefresh(iteration, exception);
                else throw exception;
            }
        }
        throw Exceptions.Default;
    }

    // Requests ---------------------------------------------------------------------------------------------------------------------------
    public static async Task<HTTPHeadResult> Head(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Head, false, url, query, headers, token: token);
    }

    public static async Task<HTTPHeadResult> Options(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Options, false, url, query, headers, token: token);
    }
    public static async Task<HTTPResult<T>> Options<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        HTTPToken? token = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Options, true, url, query, headers, token: token);
    }

    public static async Task<HTTPHeadResult> Trace(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Trace, false, url, query, headers, token: token);
    }
    public static async Task<HTTPResult<T>> Trace<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        HTTPToken? token = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Trace, true, url, query, headers, token: token);
    }

    public static async Task<HTTPHeadResult> Get(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Get, false, url, query, headers, token: token);
    }
    public static async Task<HTTPResult<T>> Get<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null,
        HTTPToken? token = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Get, true, url, query, headers, token: token);
    }

    public static async Task<HTTPHeadResult> Connect(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Connect, false, url, query, headers, contentHeaders, body, token: token);
    }
    public static async Task<HTTPResult<T>> Connect<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Connect, true, url, query, headers, contentHeaders, body, token: token);
    }

    public static async Task<HTTPHeadResult> Post(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Post, false, url, query, headers, contentHeaders, body, token: token);
    }
    public static async Task<HTTPResult<T>> Post<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Post, true, url, query, headers, contentHeaders, body, token: token);
    }

    public static async Task<HTTPHeadResult> Put(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Put, false, url, query, headers, contentHeaders, body, token: token);
    }
    public static async Task<HTTPResult<T>> Put<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Put, true, url, query, headers, contentHeaders, body, token: token);
    }

    public static async Task<HTTPHeadResult> Patch(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Patch, false, url, query, headers, contentHeaders, body, token: token);
    }
    public static async Task<HTTPResult<T>> Patch<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Patch, true, url, query, headers, contentHeaders, body, token: token);
    }

    public static async Task<HTTPHeadResult> Delete(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return await Request<object>(HttpMethod.Delete, false, url, query, headers, contentHeaders, body, token: token);
    }
    public static async Task<HTTPResult<T>> Delete<T>(
        string url,
        QueryParams? query = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? contentHeaders = null, object? body = null,
        HTTPToken? token = null
    ) {
        return (HTTPResult<T>) await Request<T>(HttpMethod.Delete, true, url, query, headers, contentHeaders, body, token: token);
    }

    // Sync -------------------------------------------------------------------------------------------------------------------------------
    private static bool IsSyncing = false;

    public static void EnforceSync()
    {
        if (AppEnvironment.IsServer || IsSyncing) return;
        throw new InvalidOperationException("This operation needs to be synced with other browsers using Sync method!");
    }

    /// <summary>Wrap any client action inside to sync across browser tabs.</summary>
    /// <param name="callback">Delegated action to synchronize.</param>
    /// <returns>Task to await.</returns>
    public static async Task Sync(Func<Task> callback) {
        await TabLock(new(async () => { 
            IsSyncing = true;
            try {
                await callback();
            }
            finally {
                IsSyncing = false;
            }
            return true; 
        }));
    }

    /// <summary>Wrap any client action inside to sync across browser tabs.</summary>
    /// <param name="callback">Delegated action to synchronize.</param>
    /// <returns>Task to await.</returns>
    public static async Task Sync(Action callback) {
        await TabLock(new(() => { 
            IsSyncing = true;
            try {
                callback();
            }
            finally {
                IsSyncing = false;
            }
            return true; 
        }));
    }

    /// <summary>Wrap any client action inside to sync across browser tabs.</summary>
    /// <param name="callback">Delegated action to synchronize.</param>
    /// <returns>Task to await with response.</returns>
    public static async Task<R> Sync<R>(Func<Task<R>> callback) {
        R? response = default;
        await TabLock(new(async () => { 
            IsSyncing = true;
            try {
                response = await callback();
            }
            finally {
                IsSyncing = false;
            }
            return true; 
        }));
        return response!;
    }

    /// <summary>Wrap any client action inside to sync across browser tabs.</summary>
    /// <param name="callback">Delegated action to synchronize.</param>
    /// <returns>Task to await with response.</returns>
    public static async Task<R> Sync<R>(Func<R> callback) {
        R? response = default;
        await TabLock(new(() => {
            IsSyncing = true;
            try {
                response = callback();
            }
            finally {
                IsSyncing = false;
            }
            return true; }));
        return response!;
    }

    // Try --------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Wrap all HTTP requests inside to sync browser tabs and respond to errors.</summary>
    /// <param name="callback">Request delegate.</param>
    /// <param name="form">Form id to display errors on.</param>
    /// <returns>A task to await that returns true if no error occurs.</returns>
    public static async Task<bool> Try(Func<Task> callback, string? form = null) {
        return await TabLock(new(async () => {
            IsSyncing = true;
            if (form != null) FormManager.ClearErrors(form);
            try {
                await callback();
                return true;
            }
            catch (AppException e) { if (e.Code != Codes.RequestCancelled) await OnError(e, form); }
            catch (AggregateException e) {
                foreach (var inner in e.InnerExceptions) {
                    if (inner is AppException app && app.Code == Codes.RequestCancelled) continue;
                    await OnError(inner, form);
                }
            }
            catch (Exception e) { await OnError(e, form); }
            finally {
                IsSyncing = false;
            }
            return false;
        }));
    }

    /// <summary>Try with no error response. (useful for requests like analytics)</summary>
    /// <param name="callback">Request delegate.</param>
    /// <returns>A task to await that returns true if no error occurs.</returns>
    public static async Task<bool> TrySilent(Func<Task> callback) {
        return await TabLock(new(async () => {
            try { 
                IsSyncing = true;
                await callback(); 
                return true;
            }
            catch { return false; }
            finally {
                IsSyncing = false;
            }
        }));
    }

    // Await ------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Use to await multiple concurrent requests.</summary>
    /// <param name="tasks">Request tasks.</param>
    /// <returns>Task to await.</returns>
    /// <exception cref="AggregateException">Aggregated exceptions of every failed request.</exception>
    public static async Task Await(Task[] tasks) {
        try {
            await Task.WhenAll(tasks);
        } 
        catch {
            var exceptions = tasks
                .Where(x => x.Exception is not null)
                .SelectMany(x => x.Exception!.InnerExceptions)
                .ToList();
            throw new AggregateException(exceptions);
        }
    }
}
