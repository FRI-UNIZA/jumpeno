namespace Jumpeno.Client.Services;

public static class Auth {    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static User User { get; private set; } = null!;
    public static Admin Admin { get; private set; } = null!;

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public static bool IsUser => User != null;
    public static bool IsRegisteredUser => IsUser && User.ID != null;
    public static bool IsAnonymousUser => IsUser && !IsRegisteredUser;
    public static bool IsAdmin => Admin != null;
    public static bool IsLoggedIn => IsRegisteredUser || IsAdmin;
    public static bool IsRole(ROLE role) {
        switch (role) {
            case ROLE.USER: return IsRegisteredUser;
            case ROLE.ADMIN: return IsAdmin;
            default: return false;
        }
    }
    public static bool IsRole(ROLE[] roles) {
        foreach (var role in roles) {
            if (IsRole(role)) return true;
        }
        return false;
    }

    // Processing -------------------------------------------------------------------------------------------------------------------------
    public static bool Processing { get; private set; } = false;
    private static void StartProcessing() => Processing = true;
    private static async Task StopProcessing() { Processing = false; await InvokeUpdate(); }

    // Login anonymous --------------------------------------------------------------------------------------------------------------------
    public static void LogInAnonymous(string name, SKIN skin) => User = new User(name, skin);
    public static void LogOutAnonymous() => User = null!;

    // Login registered -------------------------------------------------------------------------------------------------------------------
    public static async Task LogInUser(string email, string password) {
        await Window.Lock(async () => {
            Token.Data? access; try { access = Token.Access; } catch { access = null; }
            try {
                StartProcessing();
                // 1.2) Create body:
                var body = new UserLoginDTO(
                    Email: email,
                    Password: password
                );
                // 1.2) Validation:
                body.Assert();
                // 1.3) Send request:
                var response = await HTTP.Post<UserLoginDTOR>(API.BASE.USER_LOGIN, body: body);
                // 1.4) Validate response:
                response.Body.Assert();
                // 1.5) Store new access token:
                Token.StoreAccess(response.Body.AccessToken);
                // 1.6) Load profile:
                await LoadAuthProfile(false);
                // 1.7) Redirect:
                await Navigator.NavigateTo(I18N.Link<HomePage>());
                // 1.8) Reload tabs:
                Window.ReloadTabs();
            } catch {
                // 2.1) Revert access token:
                if (access == null) Token.DeleteAccess();
                else Token.StoreAccess(access.raw);
                // 2.2) Rethrow:
                throw;
            } finally {
                await StopProcessing();
            }
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    public static async Task<bool> TryLogInAdmin() {
        return await Window.Lock(async () => {
            // 1) Check environment:
            if (AppEnvironment.IsServer) return false;
            // 2) Read token:
            var q = URL.GetQueryParams();
            var token = q.GetString(TOKEN_TYPE.REFRESH.String());
            if (token == null) return false;
            // 3) Send request:
            try {
                StartProcessing();
                // 3.1.1) Create data:
                var body = new AuthRefreshDTO(
                    RefreshToken: token
                );
                // 3.1.2) Validation:
                body.Assert();
                // 3.1.3) Send request:
                var response = await HTTP.Post<AuthRefreshDTOR>(API.BASE.AUTH_REFRESH, body: body);
                // 3.1.4) Validate response:
                response.Body.Assert();
                // 3.1.6) Invalidate origin:
                await RequestInvalidate();
                // 3.1.7) Store access token:
                Token.StoreAccess(response.Body.AccessToken);
                // 3.1.8) Load profile:
                await LoadAuthProfile(false);
                // 3.1.9) Reload tabs:
                Window.ReloadTabs();
                // 3.1.10) Return success:
                return true;
            } catch (AppException e) {
                // 3.2.1) Throw only if fatal:
                CheckInvalidToken(e);
                // 3.2.2) Delete access token:
                Token.DeleteAccess();
                // 3.2.3) Reset profile:
                await ResetAuthProfile(false);
                // 3.2.4) Show notification:
                if (e.Code == CODE.INVALID_TOKEN) Notification.Error(e.Message);
                // 3.2.5) Return fail:
                return false;
            } finally {
                // 4) Update URL:
                q.Remove(TOKEN_TYPE.REFRESH.String());
                await Navigator.SetQueryParams(q);
                await StopProcessing();
            }
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    public static async Task<bool> TryLogInToken() {
        return await Window.Lock(async () => {
            try {
                StartProcessing();
                // 1.1) Request access token:
                var response = await HTTP.Post<AuthRefreshDTOR>(API.BASE.AUTH_REFRESH);
                // 1.2) Validate response:
                response.Body.Assert();
                // 1.3) Invalidate origin:
                await RequestInvalidate();
                // 1.4) Store access token:
                Token.StoreAccess(response.Body.AccessToken);
                // 1.5) Load profile:
                await LoadAuthProfile(false);
                // 1.6) Return success:
                return true;
            } catch (AppException e) {
                // 2.1) Throw only if fatal:
                CheckInvalidToken(e);
                // 2.2) Delete access token:
                Token.DeleteAccess();
                // 2.3) Reset profile:
                await ResetAuthProfile(false);
                // 2.4) Return fail:
                return false;
            } finally {
                await StopProcessing();
            }
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    public static async Task Refresh(int iteration) {
        await Window.Lock(async () => {
            try {
                StartProcessing();
                // 1.1) Check iteration:
                if (iteration > 1) throw EXCEPTION.INVALID_TOKEN;
                // 1.2) Request access token:
                var response = await HTTP.Post<AuthRefreshDTOR>(API.BASE.AUTH_REFRESH);
                // 1.3) Validate response:
                response.Body.Assert();
                // 1.4) Check if correct (across tabs):
                var data = Token.Decode(response.Body.AccessToken) ?? throw EXCEPTION.INVALID_TOKEN;
                if (data.sub != Token.Access.sub) Navigator.Refresh();
                // 1.5) Invalidate origin:
                await RequestInvalidate();
                // 1.6) Store access token:
                Token.StoreAccess(response.Body.AccessToken);
            } catch (AppException e) {
                // 2.1) Intercept not authenticated:
                CheckInvalidToken(e);
                // 2.2) Delete access token:
                Token.DeleteAccess();
                // 2.3) Reset profile:
                await ResetAuthProfile(false);
                // 2.4) Navigate to login:
                await Navigator.NavigateTo(I18N.Link<LoginPage>(), forceLoad: true);
                // 2.5) Throw back:
                throw EXCEPTION.NOT_AUTHENTICATED;
            } finally {
                await StopProcessing();
            }
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    public static async Task LogOut() {
        await Window.Lock(async () => {
            try {
                StartProcessing();
                // 1) Invalidate refresh token:
                await RequestDelete();
                // 2) Delete access token:
                Token.DeleteAccess();
                // 3) Reset profile:
                await ResetAuthProfile(false);
                // 4) Navigate to login:
                await Navigator.NavigateTo(I18N.Link<LoginPage>());
                // 5) Reload tabs:
                Window.ReloadTabs();
            } finally {
                await StopProcessing();
            }
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    // Profile ----------------------------------------------------------------------------------------------------------------------------
    // Events:
    private static event Action? UpdateEvent;
    private static event Func<Task>? UpdateEventAsync;
    private static readonly LockerSlim UpdateLock = AppEnvironment.IsClient ? new() : null!;
    private static async Task InvokeUpdate() {
        if (AppEnvironment.IsServer) return;
        UpdateEvent?.Invoke();
        if (UpdateEventAsync != null) await UpdateEventAsync.Invoke();
    }
    // Listeners:
    public static async Task AddUpdateListener(Action listener) => await (UpdateLock?.TryExclusive(() => UpdateEvent += listener) ?? Task.CompletedTask);
    public static async Task RemoveUpdateListener(Action listener) => await (UpdateLock?.TryExclusive(() => UpdateEvent -= listener) ?? Task.CompletedTask);
    public static async Task AddUpdateListener(Func<Task> listener) => await (UpdateLock?.TryExclusive(() => UpdateEventAsync += listener) ?? Task.CompletedTask);
    public static async Task RemoveUpdateListener(Func<Task> listener) => await (UpdateLock?.TryExclusive(() => UpdateEventAsync -= listener) ?? Task.CompletedTask);
    // Components:
    public static async Task Register(Component component) => await AddUpdateListener(component.Notify);
    public static bool Freezed(Component component) => Processing;
    public static bool NotFreezed(Component component) => !Freezed(component);
    public static async Task Unregister(Component component) => await RemoveUpdateListener(component.Notify);
    // Actions:
    private static async Task LoadAuthProfile(bool processing = true) {
        if (AppEnvironment.IsServer) return;
        await UpdateLock.Exclusive(async () => {
            try {
                if (processing) StartProcessing();
                if (Token.Access.role == ROLE.USER) {
                    // 1.1) Load user profile:
                    var response = await HTTP.Get<UserProfileDTOR>(API.BASE.USER_PROFILE);
                    // 1.2) Validate response:
                    response.Body.Assert();
                    // 1.3) Store user profile:
                    User = response.Body.Profile; Admin = null!;
                } else {
                    // 2.1) Store admin profile:
                    User = null!; Admin = new(Token.Access.sub);
                }
            } finally {
                if (processing) await StopProcessing();
            }
        });
    }
    public static async Task LoadProfile() {
        if (AppEnvironment.IsServer) return;
        await Window.Lock(async () => await LoadAuthProfile(), WINDOW_LOCK.AUTHENTICATION);
    }

    private static async Task ResetAuthProfile(bool processing = true) {
        if (AppEnvironment.IsServer) return;
        await UpdateLock.Exclusive(async () => {
            if (processing) StartProcessing();
            try { User = null!; Admin = null!; }
            finally { if (processing) await StopProcessing(); }
        });
    }
    public static async Task ResetProfile() {
        if (AppEnvironment.IsServer) return;
        await Window.Lock(async () => await ResetAuthProfile(), WINDOW_LOCK.AUTHENTICATION);
    }

    // Invalidation -----------------------------------------------------------------------------------------------------------------------
    private static void CheckInvalidToken(AppException e) {
        if (e.Code == CODE.INVALID_TOKEN) return;
        else throw e;
    }
    private static async Task RequestInvalidate() {
        try { await HTTP.Delete(API.BASE.AUTH_INVALIDATE); }
        catch (AppException e) { CheckInvalidToken(e); }
    }
    private static async Task RequestDelete() {
        try { await HTTP.Delete(API.BASE.AUTH_DELETE); }
        catch (AppException e) { CheckInvalidToken(e); }
    }
}
