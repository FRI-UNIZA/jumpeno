namespace Jumpeno.Client.Pages;

public partial class GameControlPage {
    public const string ROUTE_EN = "/en/game-control";
    public const string ROUTE_SK = "/sk/ovladac-hry";
    public static readonly Role[] ROLES = [Role.Admin];

    // Form -------------------------------------------------------------------------------------------------------------------------------
    private readonly string FORM = Form.Of<GameControlPage>();
    private readonly InputViewModel<string> VMCode;
    private readonly InputViewModel<string> VMName;
    private ConfirmModal DeleteConfirmModalRef = null!;
    private ConfirmModal PlayerKickConfirmModalRef = null!;

    private void MapGameControlErrors(AppException e) {
        foreach (var error in e.Errors) {
            switch (error.ID) {
                case nameof(GameControlDTO.Code): error.SetID(VMCode.ID); break;
            }
        }
    }

    private void MapGamePlayerControlErrors(AppException e) {
        foreach (var error in e.Errors) {
            switch (error.ID) {
                case nameof(GamePlayerControlDTO.Code): error.SetID(VMCode.ID); break;
                case nameof(GamePlayerControlDTO.Name): error.SetID(VMName.ID); break;
            }
        }
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GameControlPage() {
        VMCode = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(VMCode),
            TextMode: InputTextMode.UpperCase,
            Trim: true,
            TextCheck: GameValidator.IsCode,
            MaxLength: GameValidator.CODE_LENGTH,
            Placeholder: I18N.T("Code"),
            DefaultValue: ""
        ));
        VMName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(VMName),
            Trim: true,
            TextCheck: UserValidator.IsName,
            MaxLength: UserValidator.NAME_MAX_LENGTH,
            Placeholder: I18N.T("Player name"),
            DefaultValue: ""
        ));
    }

    // Actions > Game ---------------------------------------------------------------------------------------------------------------------
    private async Task ActionRequest(string url) {
        await PageLoader.Show(PageLoaderTask.GameRequest);
        await HTTP.Try(async () => {
            try {
                // 1) Data:
                var data = new GameControlDTO(){ Code = VMCode.Value };
                // 2) Validation:
                data.Assert();
                // 3) Send request:
                await HTTP.Patch(url, body: data);
            } catch (AppException e) {
                // 4) Match errors:
                MapGameControlErrors(e); throw;                
            }
        }, FORM);
        await PageLoader.Hide(PageLoaderTask.GameRequest);
    }

    private Task Delete() => DeleteConfirmModalRef.Open(() => ActionRequest(API.BASE.GAME_DELETE));

    private Task Toggle() => ActionRequest(API.BASE.GAME_TOGGLE);

    // Actions > Player -------------------------------------------------------------------------------------------------------------------
    private async Task PlayerRequest(string url) {
        await PageLoader.Show(PageLoaderTask.GameRequest);
        await HTTP.Try(async () => {
            try {
                // 1) Data:
                var data = new GamePlayerControlDTO(){ Code = VMCode.Value, Name = VMName.Value };
                // 2) Validation:
                data.Assert();
                // 3) Send request:
                await HTTP.Patch(url, body: data);
            } catch (AppException e) {
                // 4) Match errors:
                MapGamePlayerControlErrors(e); throw;
            }
        }, FORM);
        await PageLoader.Hide(PageLoaderTask.GameRequest);
    }

    private Task PlayerReady() => PlayerRequest(API.BASE.GAME_SET_PLAYER_READY);

    private async Task PlayerKick() {
        await HTTP.Try(async () => {
            // 1) Check name before modal open:
            UserValidator.AssertName(VMName.Value, checkUnknown: true, VMName.ID);
            // 2) Open confirm modal:
            await PlayerKickConfirmModalRef.Open(() => PlayerRequest(API.BASE.GAME_KICK_PLAYER));
        }, FORM);
    }
}
