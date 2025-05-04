namespace Jumpeno.Client.Services;

public static class Init {
    public static async Task<bool> TryActivate() {
        // 1) Check environment:
        if (AppEnvironment.IsServer) return false;
        // 2) Read token:
        var q = URL.GetQueryParams();
        var token = q.GetString(TOKEN_TYPE.ACTIVATION.String());
        if (token == null) return false;
        // 3) Send request:
        await PageLoader.Show(PAGE_LOADER_TASK.ACTIVATION);
        await HTTP.Try(async () => {
            // 3.1) Create data:
            var body = new UserActivateDTO(
                ActivationToken: token
            );
            // 3.2) Validation:
            body.Assert();
            // 3.3) Send request:
            var response = await HTTP.Patch<MessageDTOR>(API.BASE.USER_ACTIVATE, body: body);
            // 3.4) Show result:
            Notification.Success(response.Body.Message);
        });
        // 4) Update UI:
        q.Remove(TOKEN_TYPE.ACTIVATION.String());
        await Navigator.SetQueryParams(q);
        await PageLoader.Hide(PAGE_LOADER_TASK.ACTIVATION);
        return true;
    }

    public static async Task<bool> TryPasswordReset() {
        // 1) Check environment:
        if (AppEnvironment.IsServer) return false;
        // 2) Read token:
        var q = URL.GetQueryParams();
        var token = q.GetString(TOKEN_TYPE.PASSWORD_RESET.String());
        if (token == null) return false;
        // 3) Send request:
        await PageLoader.Show(PAGE_LOADER_TASK.PASSWORD_RESET);
        await HTTP.Try(async () => {
            // 3.1) Create body:
            var body = new UserPasswordResetDTO(
                ResetToken: token
            );
            // 3.2) Validation:
            body.Assert();
            // 3.3) Send request:
            var response = await HTTP.Patch<MessageDTOR>(API.BASE.USER_PASSWORD_RESET, body: body);
            // 3.4) Show result:
            Notification.Success(response.Body.Message);
        });
        // 4) Update UI:
        q.Remove(TOKEN_TYPE.PASSWORD_RESET.String());
        await Navigator.SetQueryParams(q);
        await PageLoader.Hide(PAGE_LOADER_TASK.PASSWORD_RESET);
        return true;
    }

    public static async Task<bool> TryAutoWatch() {
        // 1) Check if view is rendered:
        if (Page.Current is not GamePage page) return false;
        if (page.View is not ConnectBox view) return false;
        // 2) Try autowatch:
        return await view.TryAutoWatch();
    }
}
