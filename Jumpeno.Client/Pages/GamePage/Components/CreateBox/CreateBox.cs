namespace Jumpeno.Client.Components;

public partial class CreateBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Form -------------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<CreateBox>();
    private readonly InputViewModel<string> VMInputName;
    private readonly CheckBoxViewModel VMCheckBoxCode;
    public const string PARAM_CODE_INPUT = $"{GAME_HUB.PARAM_CODE}.{nameof(PARAM_CODE_INPUT)}";
    private bool VMInputCodeDisabled;
    private readonly InputViewModel<string> VMInputCode;
    private void SetVMInputCode(string urlCode) => VMInputCode.SetValue(urlCode);
    private List<SelectOption<int>> VMSelectMapOptions;
    private bool VMSelectMapDisabled;
    private LoadArea VMSelectMapLoadArea = null!;
    private SelectViewModel<int> VMSelectMap;
    private readonly SwitchViewModel VMSwitchAnonyms;
    private readonly List<SelectOption<int>> VMSelectRoundsOptions;
    private readonly SelectViewModel<int> VMSelectRounds;
    private readonly List<SelectOption<int>> VMSelectCapacityOptions;
    private readonly SelectViewModel<int> VMSelectCapacity;
    private readonly List<RadioOptionViewModel<DISPLAY_MODE_OPTION>> VMRadioDisplayModeOptions;
    private readonly RadioViewModel<DISPLAY_MODE_OPTION> VMRadioDisplayMode;
    private readonly List<RadioOptionViewModel<GAME_MODE>> VMRadioGameModeOptions;
    private readonly RadioViewModel<GAME_MODE> VMRadioGameMode;

    // Form > InitialValues ---------------------------------------------------------------------------------------------------------------
    public class InitialValuesKey : IFormInitialValuesKey { public static string Key => SESSION_STORAGE.GAME_PAGE_CREATE_BOX_FORM; }
    public class InitialValues : FormInitialValues<InitialValuesKey, InitialValues> {
        public string InputName { get; set; } = AppSettings.Name;
        public bool CheckBoxCode { get; set; } = false;
        public List<SelectOption<int>> SelectMapOptions { get; set; } = [];
        public SelectOption<int>? SelectMap { get; set; } = null;
        public bool SwitchAnonyms { get; set; } = true;
        public SelectOption<int>? SelectRounds { get; set; } = null;
        public SelectOption<int>? SelectCapacity { get; set; } = null;
        public RadioOptionDTO<DISPLAY_MODE_OPTION>? RadioDisplayMode { get; set; } = null;
        public RadioOptionDTO<GAME_MODE>? RadioGameMode { get; set; } = null;
    }
    private readonly InitialValues InitValues;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CreateBox() {
        InitValues = InitialValues.Read();
        VMInputName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: GAME_HUB.PARAM_NAME,
            Trim: true,
            TextCheck: GameValidator.IsName,
            MaxLength: GameValidator.NAME_MAX_LENGTH,
            Placeholder: I18N.T("Game name"),
            DefaultValue: InitValues.InputName,
            OnChange: new(e => InitValues.Commit(v => v.InputName = e.After))
        ));
        VMCheckBoxCode = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_CODE,
            DefaultValue: InitValues.CheckBoxCode,
            OnChange: new(e => {
                VMInputCodeDisabled = !e.Value;
                InitValues.Commit(v => v.CheckBoxCode = e.Value);
                Notify();
            })
        ));
        VMInputCodeDisabled = !InitValues.CheckBoxCode;
        VMInputCode = new(new InputViewModelTextParams(
            Form: FORM,
            ID: PARAM_CODE_INPUT,
            TextMode: INPUT_TEXT_MODE.UPPERCASE,
            Trim: true,
            TextCheck: GameValidator.IsCode,
            MaxLength: GameValidator.CODE_LENGTH,
            Placeholder: I18N.T("Code"),
            DefaultValue: ""
        ));
        VMSelectMapOptions = InitValues.SelectMapOptions;
        VMSelectMapDisabled = InitValues.SelectMapOptions.Count <= 0;
        VMSelectMap = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_MAP,
            Options: VMSelectMapOptions,
            Empty: VMSelectMapOptions.Count <= 0,
            DefaultValue: VMSelectMapOptions.Count > 0 ? InitValues.SelectMap?.Pick(o => VMSelectMapOptions[o.Key]) : null,
            OnSelect: new(e => InitValues.Commit(v => v.SelectMap = e.After)),
            Placeholder: I18N.T("Select map"),
            Search: true
        ));
        VMSwitchAnonyms = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_ANONYMS,
            DefaultValue: InitValues.SwitchAnonyms,
            OnChange: new(e => InitValues.Commit(v => v.SwitchAnonyms = e.Value))
        ));
        VMSelectRoundsOptions = [];
        for (int i = GameValidator.MIN_ROUNDS; i <= GameValidator.MAX_ROUNDS; i++) {
            VMSelectRoundsOptions.Add(new(i - GameValidator.MIN_ROUNDS, i, $"{i} {Translate.Rounds(i).ToLower()}"));
        }
        VMSelectRounds = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_ROUNDS,
            Options: VMSelectRoundsOptions,
            DefaultValue: InitValues.SelectRounds?.Pick(o => VMSelectRoundsOptions[o.Key]) ?? VMSelectRoundsOptions[2],
            OnSelect: new(e => InitValues.Commit(v => v.SelectRounds = e.After)),
            Placeholder: I18N.T("Number of rounds")
        ));
        VMSelectCapacityOptions = [];
        for (int i = GameValidator.MIN_CAPACITY; i <= GameValidator.MAX_CAPACITY; i++) {
            VMSelectCapacityOptions.Add(new(i - GameValidator.MIN_CAPACITY, i, $"{i} {Translate.Players(i).ToLower()}"));
        }
        VMSelectCapacity = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_CAPACITY,
            Options: VMSelectCapacityOptions,
            DefaultValue: InitValues.SelectCapacity?.Pick(o => VMSelectCapacityOptions[o.Key]) ?? VMSelectCapacityOptions[^1],
            OnSelect: new(e => InitValues.Commit(v => v.SelectCapacity = e.After)),
            Placeholder: I18N.T("Capacity")
        ));
        VMRadioDisplayModeOptions = [
            new(new(0, DISPLAY_MODE_OPTION.EACH_OWN, DISPLAY_MODE_OPTION.EACH_OWN.String())),
            new(new(1, DISPLAY_MODE_OPTION.ONE_SCREEN, DISPLAY_MODE_OPTION.ONE_SCREEN.String())),
            new(new(2, DISPLAY_MODE_OPTION.PRESENTATION, DISPLAY_MODE_OPTION.PRESENTATION.String()))
        ];
        VMRadioDisplayMode = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_DISPLAY_MODE,
            DefaultValue: InitValues.RadioDisplayMode?.Pick(o => VMRadioDisplayModeOptions[o.Key]) ?? VMRadioDisplayModeOptions[1],
            OnChange: new(e => InitValues.Commit(v => v.RadioDisplayMode = e.After?.DTO))
        ));
        VMRadioGameModeOptions = [
            new(new(0, GAME_MODE.MAYHEM, GAME_MODE.MAYHEM.String())),
            new(new(1, GAME_MODE.LAST_STANDING, GAME_MODE.LAST_STANDING.String()))
        ];
        VMRadioGameMode = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_GAME_MODE,
            DefaultValue: InitValues.RadioGameMode?.Pick(o => VMRadioGameModeOptions[o.Key]) ?? VMRadioGameModeOptions[0],
            OnChange: new(e => InitValues.Commit(v => v.RadioGameMode = e.After?.DTO))
        ));
    }

    protected override async Task OnComponentParametersSetAsync(bool firstTime) {
        if (!firstTime) return;
        VM.RegisterForm(FORM);
        await VM.AddURLCodeChangedListener(EventDelegate<string>.Task(SetVMInputCode));
    }

    protected override async Task OnComponentAfterRenderAsync(bool firstTime) {
        if (!firstTime) return;
        if (!Auth.IsRole(ROLE.USER)) return;
        await LoadMaps();
    }

    protected override void OnComponentDispose() {
        // TODO: Cancel map loading request
        if (VM.IsConnecting || GamePage.NavState.Get().WasCreate) return;
        InitialValues.Delete();
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        VM.UnregisterForm(FORM);
        await VM.RemoveURLCodeChangedListener(EventDelegate<string>.Task(SetVMInputCode));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task LoadMaps() {
        if (InitValues.SelectMapOptions.Count <= 0) {
            await HTTP.Try(async () => {
                // TODO: Request to load maps
                await Task.Delay(4000);
                VMSelectMapOptions = [new(0, 0, "Jumper's home"), new(1, 1, "100 Needles"), new(2, 2, "Magic temple")];
                VMSelectMap = new(new(
                    Form: FORM,
                    ID: GAME_HUB.PARAM_MAP,
                    Options: VMSelectMapOptions,
                    DefaultValue: InitValues.SelectMap?.Pick(o => VMSelectMapOptions[o.Key]),
                    OnSelect: new(e => InitValues.Commit(v => v.SelectMap = e.After)),
                    Placeholder: I18N.T("Select map"),
                    Search: true
                ));
                VMSelectMapDisabled = false;
                if (IsDisposing) return;
                InitValues.Commit(v => v.SelectMapOptions = VMSelectMapOptions);
                Notify();
            });
        }
        await VMSelectMapLoadArea.FinishLoading();
    }

    private async Task Create() {
        await PageLoader.Show(PAGE_LOADER_TASK.GAME);
        await HTTP.Try(async () => {
            // TODO: Send HTTP Create request instead
            // TODO: Cancel map loading request
            await Task.Delay(1000);
            Notification.Error(I18N.T("This functionality is not implemented yet."));
        }, FORM);
        await PageLoader.Hide(PAGE_LOADER_TASK.GAME);
    }
}
