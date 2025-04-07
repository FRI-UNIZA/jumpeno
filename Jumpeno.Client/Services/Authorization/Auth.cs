namespace Jumpeno.Client.Services;

public static class Auth {    
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static User User { get; private set; } = null!;

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public static bool IsLoggedIn() {
        return User != null;
    }

    public static bool IsRegistered() {
        return IsLoggedIn() && User!.ID != null;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------    
    public static void LogInAnonymous(string name, SKIN skin) {
        User = new User(name, skin);
    }

    public static void LogIn(Guid id, string email, string name, SKIN skin) {
        User = new User(id, email, name, skin);
    }

    public static void LogOut() {
        User = null!;
    }

    // Activation -------------------------------------------------------------------------------------------------------------------------
    public static async Task Activate() {
        // 1) Check environment:
        if (AppEnvironment.IsServer) return;
        // 2) Read token:
        var q = URL.GetQueryParams();
        var token = q.GetString(TOKEN_TYPE.ACTIVATION.String());
        if (token == null) return;
        // 3) Send request:
        await PageLoader.Show(PAGE_LOADER_TASK.ACTIVATION_REQUEST);
        await HTTP.Try(async () => {
            // 3.1) Create data:
            var body = new UserActivateDTO(
                ActivationToken: token
            );
            // 3.2) Validation:
            var errors = body.Validate();
            Checker.CheckValues(errors);
            // 3.3) Send request:
            var result = await HTTP.Patch<MessageDTOR>(API.BASE.USER_ACTIVATE, body: body);
            // 3.4) Show result:
            Notification.Success(result.Data.Message);
        });
        // 4) Show result:
        q.Remove(TOKEN_TYPE.ACTIVATION.String());
        await Navigator.SetQueryParams(q);
        await PageLoader.Hide(PAGE_LOADER_TASK.ACTIVATION_REQUEST);
    }
}
