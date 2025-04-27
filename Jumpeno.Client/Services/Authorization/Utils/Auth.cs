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

    // Login anonymous --------------------------------------------------------------------------------------------------------------------
    public static void LogInAnonymous(string name, SKIN skin) => User = new User(name, skin);
    public static void LogOutAnonymous() => User = null!;

    // Login registered -------------------------------------------------------------------------------------------------------------------
    public static async Task LogInUser(string email, string password) {
        await Window.Lock(async () => {
            Token.Data? access; try { access = Token.Access; } catch { access = null; }
            try {
                // 1.1) Create body:
                var body = new UserLoginDTO(
                    Email: email,
                    Password: password
                );
                // 1.2) Validation:
                body.Check();
                // 1.3) Send request:
                var login = await HTTP.Post<UserLoginDTOR>(API.BASE.USER_LOGIN, body: body);
                // 1.4) Validate response:
                login.Body.Check();
                // 1.5) Store new access token:
                Token.StoreAccess(login.Body.AccessToken);
                // 1.6) Load profile:
                await LoadProfile();
                // 1.7) Redirect:
                await AuthPage.NavigateTo(I18N.Link<HomePage>());
                // 1.8) Reload tabs:
                Window.ReloadTabs();
            } catch {
                // 2.1) Revert access token:
                if (access == null) Token.DeleteAccess();
                else Token.StoreAccess(access.raw);
                // 2.2) Rethrow:
                throw;
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
                // 3.1.1) Create data:
                var body = new AuthRefreshDTO(
                    RefreshToken: token
                );
                // 3.1.2) Validation:
                body.Check();
                // 3.1.3) Send request:
                var refresh = await HTTP.Post<AuthRefreshDTOR>(API.BASE.AUTH_REFRESH, body: body);
                // 3.1.4) Validate response:
                refresh.Body.Check();
                // 3.1.6) Invalidate origin:
                await RequestInvalidate();
                // 3.1.7) Store access token:
                Token.StoreAccess(refresh.Body.AccessToken);
                // 3.1.8) Load profile:
                await LoadProfile();
                // 3.1.9) Reload tabs:
                Window.ReloadTabs();
                // 3.1.10) Return success:
                return true;
            } catch (HTTPException e) {
                // 3.2.1) Throw only if fatal:
                HandleInvalidToken(e);
                // 3.2.2) Delete access token:
                Token.DeleteAccess();
                // 3.2.3) Reset profile:
                await ResetProfile();
                // 3.2.4) Show notification:
                if (e.Code == Exceptions.InvalidToken.Code) Notification.Error(e.Message);
                // 3.2.5) Return fail:
                return false;
            } finally {
                // 4) Update URL:
                q.Remove(TOKEN_TYPE.REFRESH.String());
                await Navigator.SetQueryParams(q);
            }
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    public static async Task<bool> TryLogInToken() {
        return await Window.Lock(async () => {
            try {
                // 1.1) Request access token:
                var refresh = await HTTP.Post<AuthRefreshDTOR>(API.BASE.AUTH_REFRESH);
                // 1.2) Validate response:
                refresh.Body.Check();
                // 1.3) Invalidate origin:
                await RequestInvalidate();
                // 1.4) Store access token:
                Token.StoreAccess(refresh.Body.AccessToken);
                // 1.5) Load profile:
                await LoadProfile();
                // 1.6) Return success:
                return true;
            } catch (HTTPException e) {
                // 2.1) Throw only if fatal:
                HandleInvalidToken(e);
                // 2.2) Delete access token:
                Token.DeleteAccess();
                // 2.3) Reset profile:
                await ResetProfile();
                // 2.4) Return fail:
                return false;
            }
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    public static async Task Refresh(int iteration) {
        await Window.Lock(async () => {
            try {
                // 1.1) Check iteration:
                if (iteration > 1) throw new HTTPException(Exceptions.InvalidToken.Code);
                // 1.2) Request access token:
                var refresh = await HTTP.Post<AuthRefreshDTOR>(API.BASE.AUTH_REFRESH);
                // 1.3) Validate response:
                refresh.Body.Check();
                // 1.4) Check if correct (across tabs):
                var data = Token.Decode(refresh.Body.AccessToken) ?? throw Exceptions.InvalidToken;
                if (data.sub != Token.Access.sub) Navigator.Refresh();
                // 1.5) Invalidate origin:
                await RequestInvalidate();
                // 1.6) Store access token:
                Token.StoreAccess(refresh.Body.AccessToken);
            } catch (HTTPException e) {
                // 2.1) Intercept not authenticated:
                HandleInvalidToken(e);
                // 2.2) Delete access token:
                Token.DeleteAccess();
                // 2.3) Reset profile:
                await ResetProfile();
                // 2.4) Navigate to login:
                await AuthPage.NavigateTo(I18N.Link<LoginPage>(), forceLoad: true);
                // 2.5) Throw back:
                throw Exceptions.NotAuthenticated;
            }
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    public static async Task LogOut() {
        await Window.Lock(async () => {
            // 1) Invalidate refresh token:
            await RequestDelete();
            // 2) Delete access token:
            Token.DeleteAccess();
            // 3) Reset profile:
            await ResetProfile();
            // 4) Navigate to login:
            await AuthPage.NavigateTo(I18N.Link<LoginPage>());
            // 5) Reload tabs:
            Window.ReloadTabs();
        }, WINDOW_LOCK.AUTHENTICATION);
    }

    // Profile ----------------------------------------------------------------------------------------------------------------------------
    // Events:
    private static event Action? ProfileUpdateEvent;
    private static event Func<Task>? ProfileUpdateEventAsync;
    private static readonly LockerSlim ProfileUpdateLock = AppEnvironment.IsClient ? new() : null!;
    private static async Task InvokeUpdate() {
        ProfileUpdateEvent?.Invoke();
        if (ProfileUpdateEventAsync != null) await ProfileUpdateEventAsync.Invoke();
    }
    // Listeners:
    public static async Task AddUpdateListener(Action listener) => await ProfileUpdateLock.TryExclusive(() => ProfileUpdateEvent += listener);
    public static async Task RemoveUpdateListener(Action listener) => await ProfileUpdateLock.TryExclusive(() => ProfileUpdateEvent -= listener);
    public static async Task AddUpdateListener(Func<Task> listener) => await ProfileUpdateLock.TryExclusive(() => ProfileUpdateEventAsync += listener);
    public static async Task RemoveUpdateListener(Func<Task> listener) => await ProfileUpdateLock.TryExclusive(() => ProfileUpdateEventAsync -= listener);
    
    // Actions:
    public static async Task LoadProfile() {
        await ProfileUpdateLock.TryExclusive(async () => {
            if (AppEnvironment.IsServer) return;
            if (Token.Access.role == ROLE.USER) {
                // 1.1) Load user profile:
                var profile = await HTTP.Get<UserProfileDTOR>(API.BASE.USER_PROFILE);
                // 1.2) Validate response:
                profile.Body.Check();
                // 1.3) Store user profile:
                User = profile.Body.Profile; Admin = null!;
            } else {
                // 2.1) Store admin profile:
                User = null!; Admin = new(Token.Access.sub);
            }
            await InvokeUpdate();
        });
    }
    public static async Task ResetProfile() {
        await ProfileUpdateLock.TryExclusive(async () => {
            if (AppEnvironment.IsServer) return;
            User = null!; Admin = null!;
            await InvokeUpdate();
        });
    }

    // Invalidation -----------------------------------------------------------------------------------------------------------------------
    private static void HandleInvalidToken(HTTPException e) {
        if (e.Code == Exceptions.InvalidToken.Code) return;
        else throw e;
    }
    private static async Task RequestInvalidate() {
        try { await HTTP.Delete(API.BASE.AUTH_INVALIDATE); }
        catch (HTTPException e) { HandleInvalidToken(e); }
    }
    private static async Task RequestDelete() {
        try { await HTTP.Delete(API.BASE.AUTH_DELETE); }
        catch (HTTPException e) { HandleInvalidToken(e); }
    }
}
