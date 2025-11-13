namespace Jumpeno.Client.Pages;

public partial class GameControllerPage {
    public const string ROUTE_EN = "/en/game-controller";
    public const string ROUTE_SK = "/sk/ovladac-hry";
    public static readonly ROLE[] ROLES = [ROLE.ADMIN];

    // Form -------------------------------------------------------------------------------------------------------------------------------
    private readonly string FORM = Form.Of<GameControllerPage>();
    private readonly InputViewModel<string> VMCode;
    private ConfirmModal DeleteConfirmModalRef = null!;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameControllerPage() {
        VMCode = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(GameControlDTO.Code),
            TextMode: INPUT_TEXT_MODE.UPPERCASE,
            Trim: true,
            TextCheck: GameValidator.IsCode,
            MaxLength: GameValidator.CODE_LENGTH,
            Placeholder: I18N.T("Code"),
            DefaultValue: ""
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task Request(string url) {
        await PageLoader.Show(PAGE_LOADER_TASK.GAME);
        await HTTP.Try(async () => await HTTP.Patch(url, body: new GameControlDTO(){ Code = VMCode.Value }), FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
    }
    private async Task Delete() => await DeleteConfirmModalRef.Open(async () => await Request(API.BASE.GAME_DELETE));
    private async Task Toggle() => await Request(API.BASE.GAME_TOGGLE);
}
