namespace Jumpeno.Client.Services;

public static class Auth {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static User? User { get; private set; } = null;

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void LoginAnonymous(string name, SKIN skin) {
        User = new User(Guid.NewGuid(), name, skin);
    }

    public static bool IsLoggedIn() {
        return User is not null;
    }
}
