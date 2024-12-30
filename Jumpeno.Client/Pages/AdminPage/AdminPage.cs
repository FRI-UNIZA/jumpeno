namespace Jumpeno.Client.Pages;

public partial class AdminPage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ROUTE_EN = "/en/admin";
    public const string ROUTE_SK = "/sk/admin";

    public const string PASSWORD = "FRIadm25";

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMPassword;
    
    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public AdminPage() {
        VMPassword = new(new InputViewModelTextParams(
            ID: "password",
            TextMode: INPUT_TEXT_MODE.NORMAL,
            Trim: true,
            TextCheck: Checker.IsAlphaNum,
            MaxLength: 20,
            Name: "Password",
            Label: "Password",
            Placeholder: "Password",
            DefaultValue: "",
            Secret: true
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private void Login() => StateHasChanged();
    private static async Task Start() => await GameViewModel.GameRequest(async () => await HTTP.Patch(API.BASE.GAME_START, body: Game.MOCK_CODE));
    private static async Task Pause() => await GameViewModel.GameRequest(async () => await HTTP.Patch(API.BASE.GAME_PAUSE, body: Game.MOCK_CODE));
    private static async Task Resume() => await GameViewModel.GameRequest(async () => await HTTP.Patch(API.BASE.GAME_RESUME, body: Game.MOCK_CODE));
    private static async Task Reset() => await GameViewModel.GameRequest(async () => await HTTP.Patch(API.BASE.GAME_RESET, body: Game.MOCK_CODE));    
}
