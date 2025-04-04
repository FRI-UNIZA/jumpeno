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

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private List<Person> Users = [];

    private async Task AddUser() {
        // await HTTP.Post(API.BASE.USER_CREATE, body: new {
        //     Name = "Andrej",
        //     Email = "Andrej.Vesely@gmail.com"
        // });
        await HTTP.Post(API.BASE.USER_CREATE, body: new Person() {
            Name = "Emanuel",
            Email = "Bacigala",
            Skin = "Poprad"
        });
    }

    private async Task LoadUsers() {
        Users = (await HTTP.Get<List<Person>>(API.BASE.USER_READ)).Data;
    }

    private async Task UpdateUser(int id) {
        await HTTP.Patch(API.BASE.USER_UPDATE, body: id);
    }

    private async Task DeleteUser(int id) {
        await HTTP.Delete(API.BASE.USER_DELETE, body: id);
}

    public async Task DownloadDatabase() {
        try {
            await HTTP.Get(API.BASE.DB_DOWNLOAD);
            var data = await HTTP.Download(API.BASE.DB_DOWNLOAD);
            var fileName = "Jumpeno.db";
            await JS.InvokeVoidAsync("BlazorDownloadFile", fileName, CONTENT_TYPE.X_SQLITE3, data);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }    
    }

    public async Task AdminTest() {
        await HTTP.Try(async () => {
            await HTTP.Get(API.BASE.DB_ADMIN_TEST);
        });
    }


    protected override void OnPageInitialized() => Console.WriteLine("LoginPage:OnPageInitialized");
    protected override async Task OnPageInitializedAsync() => Console.WriteLine("LoginPage:OnPageInitializedAsync");
    protected override void OnPageParametersSet(bool firstTime) => Console.WriteLine("LoginPage:OnPageParametersSet");
    protected override async Task OnPageParametersSetAsync(bool firstTime) => Console.WriteLine("LoginPage:OnPageParametersSetAsync");
    protected override void OnPageAfterRender(bool firstTime) => Console.WriteLine("LoginPage:OnPageAfterRender");
    protected override async Task OnPageAfterRenderAsync(bool firstTime) => Console.WriteLine("LoginPage:OnPageAfterRenderAsync");
    protected override void OnPageDispose() => Console.WriteLine("LoginPage:OnPageDispose");
    protected override async ValueTask OnPageDisposeAsync() => Console.WriteLine("LoginPage:OnPageDisposeAsync");
}
