namespace Jumpeno.Client.Components;

public partial class ConnectBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<ConnectBox>();

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly InputViewModel<string> VMCode;
    private async Task SetInputCode(string urlCode) => await VMCode.SetValue(urlCode);

    private readonly InputViewModel<string> VMName;
    private static string LastNameValue = "";

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ConnectBox() {
        VMCode = new(new InputViewModelTextParams(
            Form: FORM,
            ID: GAME_HUB.PARAM_CODE,
            TextMode: INPUT_TEXT_MODE.UPPERCASE,
            Trim: true,
            TextCheck: Checker.IsAlphaNum,
            MaxLength: GameValidator.CODE_LENGTH,
            Name: GAME_HUB.PARAM_CODE,
            Label: I18N.T("Game code"),
            Placeholder: I18N.T("Code"),
            DefaultValue: ""
        ));
        VMName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: GAME_HUB.PARAM_NAME,
            Trim: true,
            TextCheck: Checker.IsAlphaNum,
            MaxLength: UserValidator.NAME_MAX_LENGTH,
            Name: GAME_HUB.PARAM_NAME,
            Label: I18N.T("Your name"),
            Placeholder: I18N.T("Your name"),
            DefaultValue: "",
            OnChange: new(value => LastNameValue = value)
        ));
    }

    private readonly TaskCompletionSource InitTCS = new();

    protected override async Task OnComponentInitializedAsync() {
        await VMName.SetValue(LastNameValue == "" ? User.GenerateName() : LastNameValue);
    }

    protected override async Task OnComponentParametersSetAsync(bool firstTime) {
        if (!firstTime) return;
        VM.RegisterForm(FORM);
        await VM.AddURLCodeChangedListener(SetInputCode);
        InitTCS.TrySetResult();
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        VM.UnregisterForm(FORM);
        await VM.RemoveURLCodeChangedListener(SetInputCode);
    }

    // Auto-Watch -------------------------------------------------------------------------------------------------------------------------
    public const string WATCH_QUERY = "Watch";

    public async Task<bool> TryAutoWatch() {
        // 1) Wait for params initialization:
        await InitTCS.Task;
        // 2) Check query params:
        if (!URL.GetQueryParams().IsTrue(WATCH_QUERY)) return false;
        // 3) Remove query params:
        var q = URL.GetQueryParams();
        q.Remove(WATCH_QUERY);
        await Navigator.SetQueryParams(q);
        // 4) Check if cookie modal is displayed:
        if (!CookieStorage.CookiesAccepted) return false;
        // 5) Try connect as spectator:
        await HandleWatch();
        // 6) Return result:
        return true;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task HandlePlay() => await VM.PlayRequest(new(VMCode.Value, VMName.Value));
    private async Task HandleWatch() => await VM.WatchRequest(new(VMCode.Value, VMName.Value));
}
