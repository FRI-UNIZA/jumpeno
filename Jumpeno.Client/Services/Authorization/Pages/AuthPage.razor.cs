namespace Jumpeno.Client.Pages;

#pragma warning disable CS4014

public partial class AuthPage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string LinkLogin => I18N.Link<LoginPage>(); // User not authenticated
    public static string LinkFallback => I18N.Link<HomePage>(); // Role not allowed

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public static void Init(App app) => App = app;

    // Rendering --------------------------------------------------------------------------------------------------------------------------
    private static App App = null!;
    public static InitStage Stage { get; private set; } = InitStage.Authorizing;
    private static TaskCompletionSource StageTCS = new();
    public static Task NextStage() { try { Stage++; StageTCS = new(); return StageTCS.Task; } finally { App.Notify(); } }
    public static void FinishStage() => StageTCS.TrySetResult();
    private static bool IsRendering() {
        if (AppEnvironment.IsServer) return true;
        else return Stage == InitStage.Rendering;
    }

    // Layout -----------------------------------------------------------------------------------------------------------------------------
    private static bool IsNotBlock(RenderFragment? body) => !Auth.IsRole(Page.GetRolesBlock(body));
    private static bool IsAuthenticated(RenderFragment? body) => Page.GetRoles(body).Length <= 0 || Auth.IsLoggedIn;
    private static bool IsAuthorized(RenderFragment? body) {
        var roles = Page.GetRoles(body);
        if (roles.Length <= 0) return true;
        foreach (var role in roles) {
            if (Auth.IsRole(role)) return true;
        }
        return false;
    }

    private static RenderFragment? RenderError(Type error) => builder => {
        builder.OpenComponent(0, error);
        builder.SetKey(URL.Path());
        builder.CloseComponent();
    };

    public static RenderFragment? Render(RenderFragment? body) {
        if (!IsNotBlock(body)) return RenderError(typeof(Error403Page));
        else if (!IsAuthenticated(body)) return RenderError(typeof(Error401Page));
        else if (!IsAuthorized(body)) return RenderError(typeof(Error403Page));
        else return body;
    }

    // Redirect ---------------------------------------------------------------------------------------------------------------------------
    public static async Task ChangePath() {
        if (!AppSettings.Redirect) return;
        var q = URL.Query();
        if (!Auth.IsLoggedIn) {
            if (Page.GetRoles(URL.Path()).Length > 0) await Navigator.NavigateTo(URL.WithQuery(LinkLogin, q), replace: true);
        } else {
            if (Auth.IsRole(Page.GetRolesBlock(URL.Path()))) await Navigator.NavigateTo(URL.WithQuery(LinkFallback, q), replace: true);
            var roles = Page.GetRoles(URL.Path());
            if (roles.Length > 0 && !Auth.IsRole(roles)) await Navigator.NavigateTo(URL.WithQuery(LinkFallback, q), replace: true);
        }
    }

    public static async Task AddRedirect() {
        if (!AppSettings.Redirect) return;
        await Navigator.AddBlocker(e => {
            if (e.IsPopState) return true;
            var q = URL.Query();
            if (!Auth.IsLoggedIn) {
                if (Page.GetRoles(URL.ToRelative(e.AfterURL)).Length > 0) {
                    Navigator.NavigateTo(URL.WithQuery(LinkLogin, q));
                    return false;
                }
                return true;
            } else {
                if (Auth.IsRole(Page.GetRolesBlock(URL.ToRelative(e.AfterURL)))) {
                    Navigator.NavigateTo(URL.WithQuery(LinkFallback, q));
                    return false;
                }
                var roles = Page.GetRoles(URL.ToRelative(e.AfterURL));
                if (roles.Length > 0 && !Auth.IsRole(roles)) {
                    Navigator.NavigateTo(URL.WithQuery(LinkFallback, q));
                    return false;
                }
                return true;
            }
        });
    }
}
