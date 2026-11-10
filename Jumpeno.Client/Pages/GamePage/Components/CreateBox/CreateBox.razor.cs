namespace Jumpeno.Client.Components;

public partial class CreateBox {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IdButtonTryAgain = "button-try-again";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set("create-box", Base);

    private static CssClass LoadAreaMapOptionClass() {
        return new CssClass("load-area-map-option")
        // NOTE: Styled as field:
        .Set(FormField<SelectViewModel<int>>.ClassName)
        .Set(FormVariant.Primary.CSSClass())
        .Set(FormSize.S.CSSClass());
    }

    // Form -------------------------------------------------------------------------------------------------------------------------------
    public readonly string FormId = Form.Of<CreateBox>();
    // Name:
    private readonly InputViewModel<string> VMInputName;
    // Code:
    private readonly CheckBoxViewModel VMCheckBoxCode;
    public const string IdCodeInput = $"{nameof(GameHubCreateDTO.Code)}_{nameof(IdCodeInput)}";
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
        Form: FormId,
        ID: nameof(GameHubCreateDTO.Map),
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
        OnAfterCloseSelected: new(e => {
            GameMapSelectLoadArea.Focus();
            Async.Fire(() => LoadMap(e.After.Value));
        }),
        Placeholder: I18N.T("Select map"),
        Search: true,
        SearchMaxLength: MapValidator.NameMaxLength
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
    private readonly List<SelectOption<byte>> VMSelectRoundsOptions;
    private readonly SelectViewModel<byte> VMSelectRounds;
    // Capacity:
    private readonly List<SelectOption<byte>> VMSelectCapacityOptions;
    private readonly SelectViewModel<byte> VMSelectCapacity;
    // Display mode:
    private readonly List<RadioOptionViewModel<DisplayMode>> VMRadioDisplayModeOptions;
    private readonly List<string> VMRadioDisplayModeDescriptions = [];
    private readonly RadioViewModel<DisplayMode> VMRadioDisplayMode;
    // Game mode:
    private readonly List<RadioOptionViewModel<GameMode>> VMRadioGameModeOptions;
    private readonly List<string> VMRadioGameModeDescriptions = [];
    private readonly RadioViewModel<GameMode> VMRadioGameMode;

    // Form > InitialValues ---------------------------------------------------------------------------------------------------------------
    public class InitialValuesKey : IFormInitialValuesKey { public static string Key => SesionStorage.GamePageCreateBoxForm; }
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
        public SelectOption<byte>? SelectRounds { get; set; } = null;
        // Capacity:
        public SelectOption<byte>? SelectCapacity { get; set; } = null;
        // Display mode:
        public RadioOptionDTO<DisplayMode>? RadioDisplayMode { get; set; } = null;
        // Game mode:
        public RadioOptionDTO<GameMode>? RadioGameMode { get; set; } = null;
    }
    private readonly InitialValues InitValues;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CreateBox() {
        InitValues = InitialValues.Read();
        // Name:
        VMInputName = new(new InputViewModelTextParams(
            Form: FormId,
            ID: nameof(GameHubCreateDTO.GameName),
            Trim: true,
            TextCheck: GameValidator.IsName,
            MaxLength: GameValidator.NameMaxLength,
            Placeholder: I18N.T("Game name"),
            DefaultValue: InitValues.InputName,
            OnChange: new(e => InitValues.Commit(v => v.InputName = e.After))
        ));
        // Code:
        VMCheckBoxCode = new(new(
            Form: FormId,
            ID: nameof(GameHubCreateDTO.Code),
            DefaultValue: InitValues.CheckBoxCode,
            OnChange: new(e => {
                VMInputCodeDisabled = !e.Value;
                InitValues.Commit(v => v.CheckBoxCode = e.Value);
                Notify();
            })
        ));
        VMInputCodeDisabled = !InitValues.CheckBoxCode;
        VMInputCode = new(new InputViewModelTextParams(
            Form: FormId,
            ID: IdCodeInput,
            TextMode: InputTextMode.UpperCase,
            Trim: true,
            TextCheck: GameValidator.IsCode,
            MaxLength: GameValidator.CodeLength,
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
            Form: FormId,
            ID: nameof(GameHubCreateDTO.Anonyms),
            DefaultValue: InitValues.SwitchAnonyms,
            OnChange: new(e => InitValues.Commit(v => v.SwitchAnonyms = e.Value))
        ));
        // Rounds:
        VMSelectRoundsOptions = [];
        for (byte i = GameValidator.MinRounds; i <= GameValidator.MaxRounds; i++) {
            VMSelectRoundsOptions.Add(new(i - GameValidator.MinRounds, i, $"{i} {Translate.Rounds(i).ToLower()}"));
        }
        VMSelectRounds = new(new(
            Form: FormId,
            ID: nameof(GameHubCreateDTO.Rounds),
            Options: VMSelectRoundsOptions,
            DefaultValue: InitValues.SelectRounds?.Pick(o => VMSelectRoundsOptions[o.Key]) ?? VMSelectRoundsOptions[2],
            OnSelect: new(e => InitValues.Commit(v => v.SelectRounds = e.After)),
            Placeholder: I18N.T("Number of rounds")
        ));
        // Capacity:
        VMSelectCapacityOptions = [];
        for (byte i = GameValidator.MinCapacity; i <= GameValidator.MaxCapacity; i++) {
            VMSelectCapacityOptions.Add(new(i - GameValidator.MinCapacity, i, $"{i} {Translate.Players(i).ToLower()}"));
        }
        VMSelectCapacity = new(new(
            Form: FormId,
            ID: nameof(GameHubCreateDTO.Capacity),
            Options: VMSelectCapacityOptions,
            DefaultValue: InitValues.SelectCapacity?.Pick(o => VMSelectCapacityOptions[o.Key]) ?? VMSelectCapacityOptions[^1],
            OnSelect: new(e => InitValues.Commit(v => v.SelectCapacity = e.After)),
            Placeholder: I18N.T("Capacity")
        ));
        // Display mode:
        VMRadioDisplayModeOptions = [
            new(new(0, DisplayMode.EachOwn, DisplayMode.EachOwn.String())),
            new(new(1, DisplayMode.OneScreen, DisplayMode.OneScreen.String())),
            new(new(2, DisplayMode.Presentation, DisplayMode.Presentation.String()))
        ];
        VMRadioDisplayModeDescriptions.Add(I18N.T("Each has their own"));
        VMRadioDisplayModeDescriptions.Add(I18N.T("Play on 1 screen"));
        VMRadioDisplayModeDescriptions.Add(I18N.T("Host only presents"));
        VMRadioDisplayMode = new(new(
            Form: FormId,
            ID: nameof(GameHubCreateDTO.DisplayMode),
            DefaultValue: InitValues.RadioDisplayMode?.Pick(o => VMRadioDisplayModeOptions[o.Key]) ?? VMRadioDisplayModeOptions[1],
            OnChange: new(e => {
                InitValues.Commit(v => v.RadioDisplayMode = e.After?.DTO);
                StateHasChanged();
            })
        ));
        // Game mode:
        VMRadioGameModeOptions = [
            new(new(0, GameMode.Mayhem, GameMode.Mayhem.String())),
            new(new(1, GameMode.LastStanding, GameMode.LastStanding.String()))
        ];
        VMRadioGameModeDescriptions.Add(I18N.T("Timed game with respawns"));
        VMRadioGameModeDescriptions.Add(I18N.T("Until one player remains"));
        VMRadioGameMode = new(new(
            Form: FormId,
            ID: nameof(GameHubCreateDTO.GameMode),
            DefaultValue: InitValues.RadioGameMode?.Pick(o => VMRadioGameModeOptions[o.Key]) ?? VMRadioGameModeOptions[0],
            OnChange: new(e => {
                InitValues.Commit(v => v.RadioGameMode = e.After?.DTO);
                StateHasChanged();
            })
        ));
    }

    protected override async Task OnComponentInitializedAsync() {
        SetVMInputCode(VM.URLCode);
        VM.RegisterForm(FormId);
        await VM.AddURLCodeChangedListener(EventDelegate<string>.Task(SetVMInputCode));
    }

    protected override void OnComponentAfterRender(bool firstTime) {
        if (!firstTime) return;
        if (!Auth.IsRole(Role.User)) return;
        Async.Fire(LoadMaps);
    }

    protected override void OnComponentDispose() {
        if (VM.IsConnecting || GamePage.NavState.Get().WasCreate) return;
        InitialValues.Delete();
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        await DisposeMapRequests();
        await VM.RemoveURLCodeChangedListener(EventDelegate<string>.Task(SetVMInputCode));
        VM.UnregisterForm(FormId);
    }

    // Map --------------------------------------------------------------------------------------------------------------------------------
    private async Task LoadMaps() {
        try {
            // 1) Start loading:
            await StartLoading();
            // 2.1) Load map list:
            if (VMSelectMapOptions.Count <= 0) {
                if (await MapsToken.Reset() is not HTTPToken token) return;
                await LoadMapsRequest(token);
            }
            // 2.2) Check error:
            if (VMSelectMapError) return;
            // 2.3) Check empty:
            if (VMSelectMapOptions.Count <= 0) return;
            // 3) Load map detail:
            if (GameMap == null) {
                if (await MapToken.Reset() is not HTTPToken token) return;
                await LoadMapRequest(VMSelectMap.Value.Value, token);
            }
        } catch {
        } finally {
            // 4) Finish loading:
            await FinishLoading();
        }
    }

    private async Task LoadMap(int id) {
        try {
            // 1) Start loading:
            await StartLoading();
            // 2) Request:
            if (await MapToken.Reset() is not HTTPToken token) return;
            await LoadMapRequest(id, token);
        } finally {
            // 3) Finish loading:
            await FinishLoading();
        }
    }

    // Map > Loading ----------------------------------------------------------------------------------------------------------------------
    private async Task StartLoading() {
        await GameMapSelectLoadArea.StartLoading();
        VMSelectMapDisabled = true;
        StateHasChanged();
        await GameMapLoadArea.StartLoading();
    }

    private async Task FinishLoading() {
        // 1) Trigger child params change:
        StateHasChanged();
        await Task.Yield();
        // 2) Animation delay:
        await Task.Delay(AppTheme.TransitionSemiUltraFast);
        // 3) Finish map loading:
        GameMapLoadArea.SetRestoreID(IdButtonTryAgain);
        await GameMapLoadArea.FinishLoading(restoreFocus: true);
        VMSelectMapDisabled = VMSelectMapError || VMSelectMapOptions.Count <= 0;
        StateHasChanged();
        // 4) Finish select loading:
        if (VMSelectMapDisabled) return;
        GameMapSelectLoadArea.SetRestoreID(VMSelectMap.FormID);
        await GameMapSelectLoadArea.FinishLoading(restoreFocus: true);
    }

    // Map > Requests ---------------------------------------------------------------------------------------------------------------------
    private readonly HTTPRequestToken MapsToken = new();

    private async Task LoadMapsRequest(HTTPToken token) {
        try {
            // 0) Init:
            VMSelectMapOptions = [];
            VMSelectMapError = false;
            // 1) Show start:
            VMSelectMapUpdate();
            // 2) Request maps:
            await HTTP.Try(async () => { try {
                // 2.1) Send HTTP Request:
                var result = await HTTP.Get<GameMapsDTOR>(API.Base.GameMaps, token: token);
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

    private readonly HTTPRequestToken MapToken = new();

    private async Task LoadMapRequest(int id, HTTPToken token) {
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
                var result = await HTTP.Get<GameMapDTOR>(API.Base.GameMap, query: q, token: token);
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

    private async Task CancelMapRequests() {
        await MapsToken.Reset();
        await MapToken.Reset();
    }

    private async Task DisposeMapRequests() {
        await MapsToken.DisposeAsync();
        await MapToken.DisposeAsync();
    }

    // Create -----------------------------------------------------------------------------------------------------------------------------
    private async Task Create() {
        await PageLoader.Show(PageLoaderTask.GameConsent);
        await CancelMapRequests();
        await VM.CreateRequest(new(
            Code: VMInputCodeDisabled ? null : VMInputCode.Value,
            GameName: VMInputName.Value,
            Map: VMSelectMap.Empty ? null : VMSelectMap.Value.Value,
            Anonyms: VMSwitchAnonyms.Value,
            Rounds: VMSelectRounds.Value.Value,
            Capacity: VMSelectCapacity.Value.Value,
            DisplayMode: VMRadioDisplayMode.Value!.Value,
            GameMode: VMRadioGameMode.Value!.Value
        ));
    }
}
