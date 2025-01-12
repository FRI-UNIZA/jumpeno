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
    private readonly TaskCompletionSource InitTCS = new();

    protected override async Task OnInitializedAsync() {
        await InitAutoWatch();
        await VMName.SetValue(LastNameValue == "" ? User.GenerateName() : LastNameValue);
        if (Auth.IsRegistered()) ShowName = false;
        InitTCS.TrySetResult();
    }

    protected override async Task OnParametersSetAsync(bool firstTime) {
        if (!firstTime) return;
        await VM.AddURLCodeChangedListener(SetInputCode);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (!firstRender) return;
        await InitTCS.Task;
        await TryAutoWatch();
    }

    public async ValueTask DisposeAsync() {
        await VM.RemoveURLCodeChangedListener(SetInputCode);
        GC.SuppressFinalize(this);
    }

    // Auto-Watch -------------------------------------------------------------------------------------------------------------------------
    private static async Task InitAutoWatch() {
        if (!ConnectViewModel.RunPresentation && !ConnectViewModel.RunAutoWatch) return;
        await PageLoader.Show(PAGE_LOADER_TASK.GAME_CONNECT);
    }

    private async Task TryAutoWatch() {
        // TODO: Implement reconnect hub:
        if (ConnectViewModel.RunPresentation) await HandleWatch();
        else if (ConnectViewModel.RunAutoWatch) await HandleWatch();
    }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    private bool Validate() {
        var isValid = true;
        var errors = GameValidator.ValidateCode(VMCode.Value);
        if (errors.Count > 0) {
            VMCode.Error.SetError(I18N.T(errors[0].Message, errors[0].Values, true));
            isValid = false;
        }
        errors = User.ValidateName(VMName.Value);
        if (errors.Count > 0) {
            VMName.Error.SetError(I18N.T(errors[0].Message, errors[0].Values, true));
            isValid = false;
        }
        return isValid;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task HandlePlay() {
        if (Validate()) await VM.ConnectRequest(new(VMCode.Value, VMName.Value), false);
        await PageLoader.Hide(PAGE_LOADER_TASK.GAME_CONNECT);
    }

    private async Task HandleWatch() {
        if (Validate()) await VM.ConnectRequest(new(VMCode.Value, VMName.Value), true);
        await PageLoader.Hide(PAGE_LOADER_TASK.GAME_CONNECT);
    }
}
