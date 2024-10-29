namespace Jumpeno.Client.Components;

public partial class ConnectBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectBoxViewModel ViewModel { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private bool ShowName { get; set; } = true;
    private readonly InputViewModel<string> VMCode = new(new InputViewModelTextProps(
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

    private static string LastNameValue = "";
    private readonly InputViewModel<string> VMName = new(new InputViewModelTextProps(
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

    private bool FirstTimeParametersSet = false;
    protected override async Task OnParametersSetAsync() {
        if (FirstTimeParametersSet) return;
        FirstTimeParametersSet = true;
        await VMCode.SetValue(ViewModel.DefaultCode());
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
        await ViewModel.OnPlay.Invoke(new(VMCode.Value, VMName.Value));
    }

    private async Task HandleWatch() {
        if (!Validate()) return;
        await ViewModel.OnWatch.Invoke(new(VMCode.Value, VMName.Value));
    }
}
