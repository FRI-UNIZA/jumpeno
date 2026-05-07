namespace Jumpeno.Client.Pages;

public partial class GameControlPage {
    public const string RouteEN = "/en/game-control";
    public const string RouteSK = "/sk/ovladac-hry";
    public static readonly Role[] Roles = [Role.Admin];

    // Form -------------------------------------------------------------------------------------------------------------------------------
    private readonly string form = Form.Of<GameControlPage>();
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
            Form: form,
            ID: nameof(VMCode),
            TextMode: InputTextMode.UpperCase,
            Trim: true,
            TextCheck: GameValidator.IsCode,
            MaxLength: GameValidator.CodeLength,
            Placeholder: I18N.T("Code"),
            DefaultValue: ""
        ));
        VMName = new(new InputViewModelTextParams(
            Form: form,
            ID: nameof(VMName),
            Trim: true,
            TextCheck: UserValidator.IsName,
            MaxLength: UserValidator.NameMaxLength,
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
        }, form);
        await PageLoader.Hide(PageLoaderTask.GameRequest);
    }

    private Task Delete() => DeleteConfirmModalRef.Open(() => ActionRequest(API.Base.GameDelete));

    private Task Toggle() => ActionRequest(API.Base.GameToggle);

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
        }, form);
        await PageLoader.Hide(PageLoaderTask.GameRequest);
    }

    private Task PlayerReady() => PlayerRequest(API.Base.GameSetPlayerReady);

    private async Task PlayerKick() {
        await HTTP.Try(async () => {
            // 1) Check name before modal open:
            UserValidator.AssertName(VMName.Value, checkUnknown: true, VMName.ID);
            // 2) Open confirm modal:
            await PlayerKickConfirmModalRef.Open(() => PlayerRequest(API.Base.GameKickPlayer));
        }, form);
    }
}
