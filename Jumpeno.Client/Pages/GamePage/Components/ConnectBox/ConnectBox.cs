namespace Jumpeno.Client.Components;

public partial class ConnectBox : IAsyncDisposable {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool ShowName { get; set; } = true;
    private readonly InputViewModel<string> VMCode = new(new InputViewModelTextParams(
        ID: Game.CODE_ID,
        TextMode: INPUT_TEXT_MODE.UPPERCASE,
        Trim: true,
        TextCheck: Checker.IsAlphaNum,
        MaxLength: Game.CODE_LENGTH,
        Name: I18N.T("Game code"),
        Label: I18N.T("Game code"),
        Placeholder: I18N.T("Code"),
        DefaultValue: ""
    ));
    private async Task SetInputCode(string urlCode) => await VMCode.SetValue(urlCode);

    private static string LastNameValue = "";
    private readonly InputViewModel<string> VMName = new(new InputViewModelTextParams(
        ID: User.NAME_ID,
        Trim: true,
        TextCheck: Checker.IsAlphaNum,
        MaxLength: User.NAME_MAX_LENGTH,
        Name: I18N.T("Your name"),
        Label: I18N.T("Your name"),
        Placeholder: I18N.T("Your name"),
        DefaultValue: "",
        OnChange: new(value => LastNameValue = value)
    ));

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async Task OnInitializedAsync() {
        await VMName.SetValue(LastNameValue == "" ? User.GenerateName() : LastNameValue);
        if (Auth.IsRegistered()) ShowName = false;
    }

    protected override async Task OnParametersSetAsync(bool firstTime) {
        if (!firstTime) return;
        await VM.AddURLCodeChangedListener(SetInputCode);
    }

    public async ValueTask DisposeAsync() {
        await VM.RemoveURLCodeChangedListener(SetInputCode);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------    
    private bool Validate() {
        var isValid = true;
        var errors = Game.ValidateCode(VMCode.Value);
        if (errors.Count > 0) {
            VMCode.Error.SetError(errors[0].Message);
            isValid = false;
        }
        errors = User.ValidateName(VMName.Value);
        if (errors.Count > 0) {
            VMName.Error.SetError(errors[0].Message);
            isValid = false;
        }
        return isValid;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task HandlePlay() {
        if (!Validate()) return;
        await VM.PlayRequest(new(VMCode.Value, VMName.Value));
    }

    private async Task HandleWatch() {
        if (!Validate()) return;
        await VM.WatchRequest(new(VMCode.Value, VMName.Value));
    }
}
