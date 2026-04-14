namespace Jumpeno.Client.Pages;

public partial class LoginPage {
    public const string ROUTE_EN = "/en/login";
    public const string ROUTE_SK = "/sk/prihlasenie";
    public static readonly Role[] ROLES_BLOCK = [Role.User, Role.Admin];

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly LoginPageViewModel VM;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public LoginPage() => VM = new(this);
}
