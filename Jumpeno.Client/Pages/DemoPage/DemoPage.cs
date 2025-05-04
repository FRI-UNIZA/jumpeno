namespace Jumpeno.Client.Pages;

public partial class DemoPage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ROUTE_EN = "/en/demo";
    public const string ROUTE_SK = "/sk/demo";

    public const string PASSWORD = "FRIadm25";

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMPassword;
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public DemoPage() {
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
    private static async Task Start() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_START, body: Game.DEFAULT_CODE));
    private static async Task Pause() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_PAUSE, body: Game.DEFAULT_CODE));
    private static async Task Resume() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_RESUME, body: Game.DEFAULT_CODE));
    private static async Task Reset() => await GameViewModel.Request(async () => await HTTP.Patch(API.BASE.GAME_RESET, body: Game.DEFAULT_CODE));
}
