namespace Jumpeno.Client.Components;

public partial class DropDown {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "dropdown";
    public const string ID_START_PREFIX = "dropdown-start";
    public const string ID_BUTTON_PREFIX = "dropdown-button";
    public const string ID_OPTIONS_PREFIX = "dropdown-options";
    public const string ID_END_PREFIX = "dropdown-end";
    // Classes:
    public const string CLASS = "dropdown";
    public const string CLASS_START = "dropdown-start";
    public const string CLASS_BUTTON = "dropdown-button";
    public const string CLASS_MARK = "dropdown-mark";
    public const string CLASS_MENU = "dropdown-menu";
    public const string CLASS_OPTIONS = "dropdown-options";

    public const string CLASS_END = "dropdown-end";
    // Parameters:
    public const string PARAM_REF = "Ref";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string? ID { get; set; } = null;
    [Parameter]
    public required string Title { get; set; }
    [Parameter]
    public required RenderFragment DropDownButton { get; set; }
    [Parameter]
    public required RenderFragment Options { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string ID_DROPDOWN { get; private set; } = null!;
    public string ID_START { get; private set; } = null!;
    public string ID_BUTTON { get; private set; } = null!;
    public string ID_OPTIONS { get; private set; } = null!;
    public string ID_END { get; private set; } = null!;
    private void SetIDs(string id) {
        ID_DROPDOWN = id;
        ID_START = $"{ID_START_PREFIX}-{ID_DROPDOWN}";
        ID_BUTTON = $"{ID_BUTTON_PREFIX}-{ID_DROPDOWN}";
        ID_OPTIONS = $"{ID_OPTIONS_PREFIX}-{ID_DROPDOWN}";
        ID_END = $"{ID_END_PREFIX}-{ID_DROPDOWN}";
    }
    // Ref:
    private readonly DotNetObjectReference<DropDown> Ref;
    // State:
    private bool OpenRequested = false;
    private bool Displayed { get; set; } = false;
    private readonly LockerSlim Lock = new();
    // Computed:
    protected CSSClass ComputeClass() {
        var c = new CSSClass(CLASS);
        if (Displayed) c.Set("displayed");
        return c;
    }
    private string ComputeLabel() => Displayed ? $"{I18N.T("Close")} {Title}" : $"{I18N.T("Open")} {Title}";

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------    
    public DropDown() {
        SetIDs(IDGenerator.Generate(ID_PREFIX));
        Ref = DotNetObjectReference.Create(this);
    }

    protected override async Task OnComponentInitializedAsync() {
        if (AppEnvironment.IsClient) {
            await Window.AddClickEventListener(Ref, OnClick);
            await Window.AddScrollEventListener(Ref, OnScroll);
            await Window.AddResizeEventListener(Ref, OnResize);
            await Window.AddKeyDownEventListener(Ref, OnKeyDown);
        }
    }

    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        if (ID != null) SetIDs(ID);
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        if (AppEnvironment.IsClient) {
            await Window.RemoveClickEventListener(Ref, OnClick);
            await Window.RemoveScrollEventListener(Ref, OnScroll);
            await Window.RemoveResizeEventListener(Ref, OnResize);
            await Window.RemoveKeyDownEventListener(Ref, OnKeyDown);
        }
        Ref.Dispose();
        Lock.Dispose();
    }

    // Private actions --------------------------------------------------------------------------------------------------------------------
    private async Task Open() {
        await Lock.TryExclusive(() => {
            if (!Displayed) OpenRequested = true;
        });
    }

    private async Task Close() {
        await Lock.TryExclusive(() => {
            if (!Displayed) return;
            Displayed = false;
            SetFocus();
            StateHasChanged();
        });
    }

    // Public actions ---------------------------------------------------------------------------------------------------------------------
    public Func<Task> CreateAction(EmptyDelegate action) => async () => {
        await Lock.TryExclusive(async () => {
            Displayed = false;
            SetFocus();
            StateHasChanged();
            await action.Invoke();
        });
    };

    public void SetFocus() => ActionHandler.SetFocus(ID_BUTTON);

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task OnClick((int x, int y) position) {
        await Lock.TryExclusive(() => {
            Displayed = OpenRequested;
            OpenRequested = false;
            StateHasChanged();
        });
    }

    [JSInvokable]
    public async Task OnScroll() => await Close();

    [JSInvokable]
    public async Task OnResize(WindowResizeEvent e) => await Close();

    [JSInvokable]
    public async Task OnKeyDown(string key) { if (key == KEYBOARD.ESC) await Close(); }
}
