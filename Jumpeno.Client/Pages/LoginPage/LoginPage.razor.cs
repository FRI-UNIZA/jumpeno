namespace Jumpeno.Client.Pages;

public partial class LoginPage {
    public const string RouteEn = "/en/login";
    public const string RouteSk = "/sk/prihlasenie";
    public static readonly Role[] RolesBlock = [Role.User, Role.Admin];

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly LoginPageViewModel VM;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public LoginPage() => VM = new(this);
}
