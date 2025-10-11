namespace Jumpeno.Client.Components;

public partial class CreateBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<CreateBox>();
    private readonly InputViewModel<string> VMName;
    private readonly InputViewModel<string> VMCode;
    private readonly CheckBoxViewModel VMCustomCode;
    private readonly List<SelectOption> Maps;
    private readonly SelectViewModel VMSelectMap;
    private readonly SwitchViewModel VMSwitchAnonyms;
    private readonly List<SelectOption> Capacities;
    private readonly SelectViewModel VMSelectCapacity;
    private readonly List<RadioOptionViewModel<DISPLAY_MODE_OPTION>> VMDisplayModeOptions;
    private readonly RadioViewModel<DISPLAY_MODE_OPTION> VMDisplayMode;
    private readonly List<RadioOptionViewModel<GAME_MODE>> VMGameModeOptions;
    private readonly RadioViewModel<GAME_MODE> VMGameMode;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CreateBox() {
        VMName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: GAME_HUB.PARAM_NAME,
            Trim: true,
            TextCheck: Checker.IsAlphaNum,
            MaxLength: UserValidator.NAME_MAX_LENGTH,
            Placeholder: "Názov hry",
            DefaultValue: AppSettings.Name
        ));
        VMCode = new(new InputViewModelTextParams(
            Form: FORM,
            ID: GAME_HUB.PARAM_CODE,
            TextMode: INPUT_TEXT_MODE.UPPERCASE,
            Trim: true,
            TextCheck: Checker.IsAlphaNum,
            MaxLength: GameValidator.CODE_LENGTH,
            Placeholder: I18N.T("Code"),
            DefaultValue: ""
        ));
        VMCustomCode = new(new(
            Form: FORM,
            ID: "test",
            DefaultValue: false
        ));
        Maps = [new(0, "Jumper's home"), new(1, "Tile rain"), new(2, "Magic temple")];
        VMSelectMap = new(new(
            Form: FORM,
            ID: "map",
            Options: Maps,
            DefaultValue: Maps[0],
            Placeholder: "Zvolit mapu",
            Search: true
        ));
        VMSwitchAnonyms = new(new(
            Form: FORM,
            ID: "anonyms",
            DefaultValue: true
        ));
        Capacities = [];
        for (int i = GameValidator.MIN_CAPACITY; i <= GameValidator.MAX_CAPACITY; i++) {
            var suffix = i < 5 ? I18N.T("players2+") : I18N.T("players5+");
            Capacities.Add(new(i, $"{i} {suffix}"));
        }
        VMSelectCapacity = new(new(
            Form: FORM,
            ID: "capacity",
            Options: Capacities,
            DefaultValue: Capacities[^1],
            Placeholder: "Kapacita"
        ));
        VMDisplayModeOptions = [
            new(new(DISPLAY_MODE_OPTION.EACH_OWN, "Each own")),
            new(new(DISPLAY_MODE_OPTION.ONE_SCREEN, "One screen")),
            new(new(DISPLAY_MODE_OPTION.PRESENTATION, "Presentation"))
        ];
        VMDisplayMode = new(new(
            Form: FORM,
            ID: "display-mode",
            DefaultValue: VMDisplayModeOptions[1]
        ));
        VMGameModeOptions = [
            new(new(GAME_MODE.MAYHEM, "Mayhem")),
            new(new(GAME_MODE.LAST_STANDING, "Last standing"))
        ];
        VMGameMode = new(new(
            Form: FORM,
            ID: "game-mode",
            DefaultValue: VMGameModeOptions[0]
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Create() => Notification.Error(I18N.T("This functionality is not implemented yet."));
}
