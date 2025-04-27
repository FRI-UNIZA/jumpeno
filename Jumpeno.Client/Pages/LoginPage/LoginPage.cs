namespace Jumpeno.Client.Pages;

public partial class LoginPage {
    public const string ROUTE_EN = "/en/login";
    public const string ROUTE_SK = "/sk/prihlasenie";
    public static readonly ROLE[] ROLES_BLOCK = [ROLE.USER, ROLE.ADMIN];

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly LoginPageViewModel VM;
    public void Notify() => StateHasChanged();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public LoginPage() => VM = new(this);
}
