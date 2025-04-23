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

    protected override void OnPageInitialized() => Console.WriteLine("LoginPage:OnPageInitialized");
    protected override async Task OnPageInitializedAsync() => Console.WriteLine("LoginPage:OnPageInitializedAsync");
    protected override void OnPageParametersSet(bool firstTime) => Console.WriteLine("LoginPage:OnPageParametersSet");
    protected override async Task OnPageParametersSetAsync(bool firstTime) => Console.WriteLine("LoginPage:OnPageParametersSetAsync");
    protected override void OnPageAfterRender(bool firstTime) => Console.WriteLine("LoginPage:OnPageAfterRender");
    protected override async Task OnPageAfterRenderAsync(bool firstTime) => Console.WriteLine("LoginPage:OnPageAfterRenderAsync");
    protected override void OnPageDispose() => Console.WriteLine("LoginPage:OnPageDispose");
    protected override async ValueTask OnPageDisposeAsync() => Console.WriteLine("LoginPage:OnPageDisposeAsync");
}
