namespace Jumpeno.Client.Pages;

public partial class LoginPage {
    public const string ROUTE_EN = "/en/login";
    public const string ROUTE_SK = "/sk/prihlasenie";

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly LoginPageViewModel VM;
    public void Notify() => StateHasChanged();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public LoginPage() => VM = new(this);

    // protected override void OnPageInitialized() {
    //     var q = URL.GetQueryParams();
    //     var token = q.GetString(nameof(AccessToken));
    //     if (token == null) return;
    //     VM.Show(LOGIN_FORM.ADMIN);
    // }

    protected override void OnPageInitialized() => Console.WriteLine("LoginPage:OnPageInitialized");
    protected override async Task OnPageInitializedAsync() {
        Console.WriteLine("LoginPage:OnPageInitializedAsync");
    }
    protected override void OnPageParametersSet(bool firstTime) => Console.WriteLine("LoginPage:OnPageParametersSet");
    protected override async Task OnPageParametersSetAsync(bool firstTime) => Console.WriteLine("LoginPage:OnPageParametersSetAsync");
    protected override void OnPageAfterRender(bool firstTime) => Console.WriteLine("LoginPage:OnPageAfterRender");
    protected override async Task OnPageAfterRenderAsync(bool firstTime) {
        Console.WriteLine("LoginPage:OnPageAfterRenderAsync");
        // if (AppEnvironment.IsServer || URL.Path() == AuthPage.LINK_LOGIN) return;
        // JS.
        // await Navigator.NavigateTo(AuthPage.LINK_LOGIN, replace: true);
    }
    protected override void OnPageDispose() => Console.WriteLine("LoginPage:OnPageDispose");
    protected override async ValueTask OnPageDisposeAsync() => Console.WriteLine("LoginPage:OnPageDisposeAsync");
}
