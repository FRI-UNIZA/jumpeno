namespace Jumpeno.Client.Components;

public partial class AdminLoginForm {
    // Parameters -------------------------------------------------------------------------------------------------------------------------    
    [Parameter]
    public required LoginPageViewModel VM { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool Verified = false;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMEmail;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public AdminLoginForm() {
        VMEmail = new(new InputViewModelTextParams(
            ID: nameof(LogInAdminDTO.Email),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsEmail,
            MaxLength: 100,
            Name: nameof(LogInAdminDTO.Email),
            Label: I18N.T("Email address verification"),
            Placeholder: @I18N.T("Email"),
            DefaultValue: ""
        ));
    }

    protected override async Task OnInitializedAsync() {
        if (!AppEnvironment.IsClient) return;
        var q = URL.GetQueryParams();
        var token = q.GetString(nameof(AccessToken));
        if (token == null) return;
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN_REQUEST);
        await HTTP.Try(async () => {
            // var info = AccessToken.Decode(token);
            // if (info == null) return;
            // Console.WriteLine($"Token query: {token}");
            // foreach (var data in info) {
            //     Console.WriteLine($"data: {data.Key} | {data.Value}");
            // }
            // Console.WriteLine(info[ClaimTypes.Role]);
            // if (info[ClaimTypes.Role].ToString() != ROLE.ADMIN.ToString()) return;
            // LocalStorage.Set(nameof(AccessToken), token);
            // await HTTP.Get(API.BASE.USER_PROFILE_ADMIN);
            // await Navigator.NavigateTo(I18N.Link<HomePage>());
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN_REQUEST);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Login() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN_REQUEST);
        await HTTP.Try(async () => {
            // 1) Create data:
            var data = new LogInAdminDTO(
                Email: VMEmail.Value
            );
            // 2) Validation:
            data.Check();
            // 3) Send request:
            var response = await HTTP.Post<LogInAdminDTOR>(API.BASE.USER_LOGIN_ADMIN, body: data);
            // 4) Show result:
            Notification.Success(response.Data.Message);
            Verified = true;
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN_REQUEST);
    }
}
