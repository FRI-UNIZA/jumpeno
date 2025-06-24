namespace Jumpeno.Client.Components;

public partial class CreateBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<ConnectBox>();
    private readonly InputViewModel<string> VMCode;
    private readonly SelectViewModel VMSelectCapacity;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CreateBox() {
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
        VMSelectCapacity = new(new(
            Form: FORM,
            ID: GAME_HUB.PARAM_CAPACITY,
            Options: new Func<List<SelectOption>>(() => {
                var options = new List<SelectOption>();
                for (int i = 2; i <= 10; i++) options.Add(new(i, $"{i} {Format.OfPlayers(i)}"));
                return options;
            })(),
            DefaultValue: new SelectOption(10, $"{10} {Format.OfPlayers(10)}")
        ));
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Create() {
        await HTTP.Try(async () => {
            List<Error> errors = [];
            errors.Add(ERROR.FORMAT.SetID(VMCode.ID));
            errors.Add(ERROR.FORMAT.SetID(VMSelectCapacity.ID));
            throw EXCEPTION.CLIENT.SetInfo("Nespravne hodnoty").SetErrors(errors);
            Console.WriteLine($"Selected: {VMSelectCapacity.Value.Value} | {VMSelectCapacity.Value.Label}");
            Notification.Error(I18N.T("This functionality is not implemented yet."));
        }, FORM);
    }
}
