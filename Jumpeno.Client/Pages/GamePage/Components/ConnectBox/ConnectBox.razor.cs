namespace Jumpeno.Client.Components;

public partial class ConnectBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set("connect-box", Base);

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly string FORM = Form.Of<ConnectBox>();
    // Code:
    private readonly InputViewModel<string> VMCode;
    private void SetInputCode(string urlCode) => VMCode.SetValue(urlCode);
    // Name:
    private readonly InputViewModel<string> VMName;
    private static string LastNameValue = "";

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public ConnectBox() {
        VMCode = new(new InputViewModelTextParams(
            Form: FORM,
            ID: Auth.IsRegisteredUser ? nameof(GameHubRegisteredDTO.Code) : nameof(GameHubAnonymousDTO.Code),
            TextMode: InputTextMode.UpperCase,
            Trim: true,
            TextCheck: GameValidator.IsCode,
            MaxLength: GameValidator.CODE_LENGTH,
            Placeholder: I18N.T("Code"),
            DefaultValue: ""
        ));
        VMName = new(new InputViewModelTextParams(
            Form: FORM,
            ID: nameof(GameHubAnonymousDTO.Name),
            Trim: true,
            TextCheck: UserValidator.IsName,
            MaxLength: UserValidator.NAME_MAX_LENGTH,
            Placeholder: I18N.T("Your name"),
            DefaultValue: "",
            OnChange: new(e => LastNameValue = e.After)
        ));
    }

    private readonly TaskCompletionSource InitTCS = new();

    protected override async Task OnComponentInitializedAsync() {
        LastNameValue = LastNameValue == "" ? User.GenerateName() : LastNameValue;
        SetInputCode(VM.URLCode);
        VMName.SetValue(LastNameValue);
        VM.RegisterForm(FORM);
        await VM.AddURLCodeChangedListener(EventDelegate<string>.Task(SetInputCode));
        InitTCS.TrySetResult();
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        VM.UnregisterForm(FORM);
        await VM.RemoveURLCodeChangedListener(EventDelegate<string>.Task(SetInputCode));
    }

    // Auto-Watch -------------------------------------------------------------------------------------------------------------------------
    public const string WATCH_QUERY = "Watch";
    private bool AutoWatch = false;

    public async Task<bool> InitAutoWatch() {
        // 1) Wait for params initialization:
        await InitTCS.Task;
        // 2) Check query params:
        if (!URL.GetQueryParams().IsTrue(WATCH_QUERY)) return false;
        // 3) Remove query params:
        var q = URL.GetQueryParams();
        q.Remove(WATCH_QUERY);
        await Navigator.SetQueryParams(q);
        // 4) Check if cookie modal is displayed:
        if (RequestStorage.Get<CookieModal>(RequestStorages.COOKIE_MODAL)!.IsOpened) return false;
        // 5) Set AutoWatch:
        AutoWatch = true;
        // 6) Show loader:
        await PageLoader.Show(PageLoaderTask.GameConsent);
        // 7) Return result:
        return true;
    }

    public async Task<bool> TryAutoWatch() {
        // 1) Check AutoWatch:
        if (!AutoWatch) return false;
        // 2) Try connect as spectator:
        await HandleWatch();
        // 3) Return result:
        return true;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private async Task HandlePlay() => await VM.ConnectRequest(new(VMCode.Value, VMName.Value, false));
    private async Task HandleWatch() => await VM.ConnectRequest(new(VMCode.Value, VMName.Value, true));
}
