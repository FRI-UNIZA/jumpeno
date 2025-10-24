namespace Jumpeno.Client.Components;

public partial class CreateBox {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_BUTTON_TRY_AGAIN = "button-try-again"; 

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Form -------------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<CreateBox>();
    // Name:
    private readonly InputViewModel<string> VMInputName;
    // Code:
    private readonly CheckBoxViewModel VMCheckBoxCode;
    public const string PARAM_CODE_INPUT = $"{GAME_HUB.PARAM_CODE}.{nameof(PARAM_CODE_INPUT)}";
    private readonly InputViewModel<string> VMInputCode;
    private bool VMInputCodeDisabled;
    private void SetVMInputCode(string urlCode) => VMInputCode.SetValue(urlCode);
    // Map select:
    private readonly LoadAreaViewModel GameMapSelectLoadArea = new(loading: true);
    private List<SelectOption<int>> VMSelectMapOptions;
    private SelectViewModel<int> VMSelectMap;
    private bool VMSelectMapError;
    private bool VMSelectMapDisabled;
    private SelectViewModel<int> VMSelectMapInit() => new(new(
        Form: FORM,
        ID: GAME_HUB.PARAM_MAP,
        Options: VMSelectMapOptions,
        Empty: VMSelectMapOptions.Count <= 0,
        DefaultValue: VMSelectMapOptions.Count > 0 ? InitValues.SelectMap?.Pick(o => VMSelectMapOptions[o.Key]) : null,
        OnSelect: new(async e => {
            // 0) Commit:
            InitValues.Commit(v => v.SelectMap = e.After);
            // 1) Reset map:
            GameMap = null;
            GameMapError = false;
            // 2) Start loading:
            await StartLoading();
            // 3) Load map in OnCloseSelected!
        }),
        OnCloseSelected: new(e => {
            GameMapSelectLoadArea.Focus();
            LoadMap(e.After.Value);
        }),
        Placeholder: I18N.T("Select map"),
        Search: true
    ));
    private void VMSelectMapUpdate() {
        if (IsDisposing) return;
        InitValues.Commit(v => v.SelectMapOptions = VMSelectMapOptions);
        VMSelectMap = VMSelectMapInit();
        StateHasChanged();
    }
    // Map canvas:
    private Map? GameMap = null;
    private bool GameMapError = false;
    private readonly LoadAreaViewModel GameMapLoadArea = new(loading: true);
    private void GameMapUpdate() {
        if (IsDisposing) return;
        InitValues.Commit(v => v.GameMap = GameMap);
        StateHasChanged();
    }
    // Anonyms:
    private readonly SwitchViewModel VMSwitchAnonyms;
    // Rounds:
    private readonly List<SelectOption<int>> VMSelectRoundsOptions;
    private readonly SelectViewModel<int> VMSelectRounds;
    // Capacity:
    private readonly List<SelectOption<int>> VMSelectCapacityOptions;
    private readonly SelectViewModel<int> VMSelectCapacity;
    // Display mode:
    private readonly List<RadioOptionViewModel<DISPLAY_MODE_OPTION>> VMRadioDisplayModeOptions;
    private readonly RadioViewModel<DISPLAY_MODE_OPTION> VMRadioDisplayMode;
    // Game mode:
    private readonly List<RadioOptionViewModel<GAME_MODE>> VMRadioGameModeOptions;
    private readonly RadioViewModel<GAME_MODE> VMRadioGameMode;

    // Form > InitialValues ---------------------------------------------------------------------------------------------------------------
    public class InitialValuesKey : IFormInitialValuesKey { public static string Key => SESSION_STORAGE.GAME_PAGE_CREATE_BOX_FORM; }
    public class InitialValues : FormInitialValues<InitialValuesKey, InitialValues> {
        // Name:
        public string InputName { get; set; } = AppSettings.Name;
        // Code:
        public bool CheckBoxCode { get; set; } = false;
        // Map select:
        public List<SelectOption<int>> SelectMapOptions { get; set; } = [];
        public SelectOption<int>? SelectMap { get; set; } = null;
        // Map canvas:
        public Map? GameMap { get; set; } = null;
        // Anonyms:
        public bool SwitchAnonyms { get; set; } = true;
        // Rounds:
        public SelectOption<int>? SelectRounds { get; set; } = null;
        // Capacity:
        public SelectOption<int>? SelectCapacity { get; set; } = null;
        // Display mode:
        public RadioOptionDTO<DISPLAY_MODE_OPTION>? RadioDisplayMode { get; set; } = null;
        // Game mode:
        public RadioOptionDTO<GAME_MODE>? RadioGameMode { get; set; } = null;
    }
    private readonly InitialValues InitValues;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CreateBox() {
        InitValues = InitialValues.Read();
        // Name:
        VMInputName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: GAME_HUB.PARAM_GAME_NAME,
            Trim: true,
            TextCheck: GameValidator.IsName,
            MaxLength: GameValidator.NAME_MAX_LENGTH,
            Placeholder: I18N.T("Game name"),
            DefaultValue: InitValues.InputName,
            OnChange: new(e => InitValues.Commit(v => v.InputName = e.After))
        ));
        // Code:
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
            DefaultValue: "",
            OnInput: new(e => VMCheckBoxCode.Error.Clear())
        ));
        // Map select:
        VMSelectMapOptions = InitValues.SelectMapOptions;
        VMSelectMapDisabled = InitValues.SelectMapOptions.Count <= 0;
        VMSelectMap = VMSelectMapInit();
        // Map canvas:
        GameMap = InitValues.GameMap;
        // Anonyms:
        VMSwitchAnonyms = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_ANONYMS,
            DefaultValue: InitValues.SwitchAnonyms,
            OnChange: new(e => InitValues.Commit(v => v.SwitchAnonyms = e.Value))
        ));
        // Rounds:
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
        // Capacity:
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
        // Display mode:
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
        // Game mode:
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
        if (VM.IsConnecting || GamePage.NavState.Get().WasCreate) return;
        InitialValues.Delete();
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        await CancelMapRequests();
        await VM.RemoveURLCodeChangedListener(EventDelegate<string>.Task(SetVMInputCode));
        VM.UnregisterForm(FORM);
    }

    // Map --------------------------------------------------------------------------------------------------------------------------------
    private async Task LoadMaps() {
        try {
            // 0) Start loading:
            await StartLoading();
            // 1.1) Load map list:
            if (VMSelectMapOptions.Count <= 0) await LoadMapListRequest();
            // 1.2) Check error:
            if (VMSelectMapError) { return; }
            // 1.3) Check empty:
            if (VMSelectMapOptions.Count <= 0) { return; }
            // 2.1) Load map detail:
            if (GameMap == null) await LoadMapRequest(VMSelectMap.Value.Value);
        } catch {
        } finally {
            // 3) Finish loading:
            await FinishLoading();
        }
    }

    private async void LoadMap(int id) {
        try {
            // 0) Start loading:
            await StartLoading();
            // 1) Request:
            try { await LoadMapRequest(id); } catch {}
            // 2) Finish loading:
            await FinishLoading();
        } catch {}
    }

    // Map > Loading ----------------------------------------------------------------------------------------------------------------------
    private async Task StartLoading() {
        await GameMapSelectLoadArea.StartLoading();
        VMSelectMapDisabled = true;
        StateHasChanged();
        await GameMapLoadArea.StartLoading();
    }

    private async Task FinishLoading() {
        GameMapLoadArea.SetRestoreID(ID_BUTTON_TRY_AGAIN);
        await GameMapLoadArea.FinishLoading(restoreFocus: true);
        VMSelectMapDisabled = VMSelectMapError || VMSelectMapOptions.Count <= 0;
        StateHasChanged();
        if (VMSelectMapDisabled) return;
        GameMapSelectLoadArea.SetRestoreID(VMSelectMap.FormID);
        await GameMapSelectLoadArea.FinishLoading(restoreFocus: true);
    }

    // Map > Requests ---------------------------------------------------------------------------------------------------------------------
    private async Task LoadMapListRequest() {
        try {
            // 0) Init:
            VMSelectMapOptions = [];
            VMSelectMapError = false;
            // 1) Show start:
            VMSelectMapUpdate();
            // 2) Request maps:
            await HTTP.Try(async () => { try {
                // 2.1) Send HTTP Request:
                var result = await HTTP.Get<GameMapsDTOR>(API.BASE.GAME_MAPS);
                var body = result.Body.Assert();
                VMSelectMapOptions = [];
                for (int i = 0; i < body.Maps.Count; i++) {
                    var map = body.Maps[i];
                    VMSelectMapOptions.Add(new(i, map.ID, map.Name));
                }
                VMSelectMapError = false;
            } catch {
                // 2.2) Set error:
                VMSelectMapOptions = [];
                VMSelectMapError = true;
                if (!IsDisposing) throw;
            }});
            // 3) Show result:
            VMSelectMapUpdate();
        } catch {}
    }

    private async Task LoadMapRequest(int id) {
        try {
            // 0) Init:
            GameMap = null;
            GameMapError = false;
            // 1) Show start:
            GameMapUpdate();
            // 2) Request map:
            await HTTP.Try(async () => { try {
                // 2.1) Send HTTP request:
                QueryParams q = new(); q.Set(nameof(GameMapDTO.ID), id);
                var result = await HTTP.Get<GameMapDTOR>(API.BASE.GAME_MAP, query: q);
                var body = result.Body.Assert();
                GameMap = body.Map;
                GameMapError = false;
            } catch {
                // 2.2) Set error:
                GameMap = null;
                GameMapError = true;
                if (!IsDisposing) throw;
            }});
            // 3) Show result:
            GameMapUpdate();
        } catch {}
    }

    private static async Task CancelMapRequests() {
        await HTTP.Cancel(HttpMethod.Get, API.BASE.GAME_MAPS);
        await HTTP.Cancel(HttpMethod.Get, API.BASE.GAME_MAP);
    }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    private async Task Create() {
        await PageLoader.Show(PAGE_LOADER_TASK.GAME);
        await CancelMapRequests();
        // TODO: Create game instead:
        await VM.PlayRequest(new(Game.DEFAULT_CODE, ""));
    }
}
