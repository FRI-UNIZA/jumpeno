namespace Jumpeno.Client.Pages;

public partial class LoginPage {
    public const string RouteEN = "/en/login";
    public const string RouteSK = "/sk/prihlasenie";
    public static readonly Role[] RolesBlock = [Role.User, Role.Admin];

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly LoginPageViewModel VM;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public LoginPage() => VM = new(this);
}
