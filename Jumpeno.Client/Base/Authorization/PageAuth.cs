namespace Jumpeno.Client.Base;

public static class PageAuth {
    public static bool CanRender() {
        if (AppEnvironment.IsClient) return true;
        var attr = Page.CurrentPage.GetType().GetField("ROLES");
        if (attr == null) return true;
        ROLE[] roles = (ROLE[]) attr!.GetValue(null)!;
        if (roles.Length > 0) return false;
        else return true;
    }

    public static async Task CheckRoles(Page page) {
        if (AppEnvironment.IsServer) return;
        var attr = page.GetType().GetField("ROLES");
        if (attr == null) return;
        ROLE[] roles = (ROLE[]) attr!.GetValue(null)!;
        if (roles.Length <= 0) return;
        try {
            if (AuthToken.role == null) return;
        } catch {
            await Navigator.NavigateTo(I18N.Link<LoginPage>(), replace: true);
        }
    }

    public static bool IsAuthorized(Page page) {
        if (AppEnvironment.IsServer) return true;
        var attr = page.GetType().GetField("ROLES");
        if (attr == null) return true;
        ROLE[] roles = (ROLE[]) attr!.GetValue(null)!;
        if (roles.Length <= 0) return true;
        
        try { foreach (var role in roles) {
            if (AuthToken.role == role) return true;
        } } catch {}
        return false;
    }
}
