namespace Jumpeno.Client.Pages;

public partial class LoginPage {
    public const string ROUTE_EN = "/en/login";
    public const string ROUTE_SK = "/sk/prihlasenie";

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
            await JS.InvokeVoidAsync("BlazorDownloadFile", fileName, "application/x-sqlite3", data);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }    
    }
}
