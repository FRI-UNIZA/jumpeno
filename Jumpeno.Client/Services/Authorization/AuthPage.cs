namespace Jumpeno.Client.Services;

#pragma warning disable CS4014

public static class AuthPage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string LINK_FALLBACK => I18N.Link<HomePage>();
    public static string LINK_LOGIN => I18N.Link<LoginPage>();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public static void Init(App app) => App = app;

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    private static App App = null!;
    public static INIT_STAGE Stage { get; private set; } = INIT_STAGE.AUTHORIZATION;
    private static TaskCompletionSource StageTCS = new();
    public static Task NextStage() { try { Stage++; StageTCS = new(); return StageTCS.Task; } finally { App.Notify(); } }
    public static void FinishStage() => StageTCS.TrySetResult();
    public static bool IsRendering() {
        if (AppEnvironment.IsServer) return true;
        else return Stage == INIT_STAGE.RENDERING;
    }
    
    // Roles ------------------------------------------------------------------------------------------------------------------------------
    private static ROLE[] PageRoles(Type? pageType) {
        if (pageType == null) return [];
        var attr = pageType.GetField("ROLES");
        if (attr == null) return [];
        ROLE[]? roles = (ROLE[]?) attr.GetValue(null);
        return roles ?? [];
    }

    private static ROLE[] PageRoles(RenderFragment? body) {
        if (body == null) return [];
        return PageRoles(Page.Type(body));
    }

    // Authorization ----------------------------------------------------------------------------------------------------------------------
    public static bool IsAuthenticated(RenderFragment? body) {
        return PageRoles(body).Length <= 0 || Auth.IsLoggedIn;
    }

    public static bool IsAuthorized(RenderFragment? body) {
        var roles = PageRoles(body);
        if (roles.Length <= 0) return true;
        foreach (var role in roles) {
            if (Auth.IsRole(role)) return true;
        }
        return false;
    }

    // Login path -------------------------------------------------------------------------------------------------------------------------
    public static async Task ChangeLoginPath() {
        if (Auth.IsLoggedIn && URL.Path() == LINK_LOGIN)
        await Navigator.NavigateTo(LINK_FALLBACK, replace: true);
    }

    public static async Task AddLoginRedirect() {
        await Navigator.AddBlocker(ev => {
            if (!Auth.IsLoggedIn) return true;
            else if (URL.ToRelative(ev.AfterURL) != LINK_LOGIN) return true;
            Navigator.NavigateTo(LINK_FALLBACK);
            return false;
        });
    }
}
