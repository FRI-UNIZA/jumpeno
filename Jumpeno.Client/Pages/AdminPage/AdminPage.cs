namespace Jumpeno.Client.Pages;

public partial class AdminPage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ROUTE_EN = "/en/admin";
    public const string ROUTE_SK = "/sk/admin";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool Verified = false;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMEmail;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public AdminPage() {
        VMEmail = new(new InputViewModelTextParams(
            ID: nameof(AdminLoginDTO.Email),
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsEmail,
            MaxLength: 100,
            Name: nameof(AdminLoginDTO.Email),
            Label: I18N.T("Email address verification"),
            Placeholder: "Email",
            DefaultValue: ""
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Login() {
        await PageLoader.Show(PAGE_LOADER_TASK.LOGIN_REQUEST);
        await HTTP.Try(async () => {
            // 1) Create data:
            var data = new AdminLoginDTO(
                Email: VMEmail.Value
            );
            // 2) Validation:
            data.Check();
            // 3) Send request:
            var response = await HTTP.Post<MessageDTOR>(API.BASE.ADMIN_LOGIN, body: data);
            // 4) Show result:
            Notification.Success(response.Data.Message);
            Verified = true;
        });
        await PageLoader.Hide(PAGE_LOADER_TASK.LOGIN_REQUEST);
    }
    private static async Task Start() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_START, body: Game.DEFAULT_CODE));
    private static async Task Pause() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_PAUSE, body: Game.DEFAULT_CODE));
    private static async Task Resume() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_RESUME, body: Game.DEFAULT_CODE));
    private static async Task Reset() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_RESET, body: Game.DEFAULT_CODE));
}
