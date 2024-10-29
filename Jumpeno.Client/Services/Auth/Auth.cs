namespace Jumpeno.Client.Services;

public static class Auth {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static User? User { get; private set; } = null;

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

    public static void LogIn(Guid id, string name, SKIN skin) {
        User = new User(id, name, skin);
    }

    public static void LogOut() {
        User = null;
    }
}
