namespace Jumpeno.Client.Components;

public partial class DropDown {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IdPrefix = "dropdown";
    public const string IdStartPrefix = "dropdown-start";
    public const string IdButtonPrefix = "dropdown-button";
    public const string IdOptionsPrefix = "dropdown-options";
    public const string IdEndPrefix = "dropdown-end";
    // Classes:
    public const string ClassName = "dropdown";
    public const string ClassStart = "dropdown-start";
    public const string ClassButton = "dropdown-button";
    public const string ClassMark = "dropdown-mark";
    public const string ClassMenu = "dropdown-menu";
    public const string ClassOptions = "dropdown-options";
    public const string ClassEnd = "dropdown-end";
    public const string ClassDisplayed = "displayed";
    // Cascade:
    public const string CascadeRef = $"{nameof(DropDown)}.{nameof(CascadeRef)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string? Id { get; set; } = null;
    [Parameter]
    public required string Title { get; set; }
    [Parameter]
    public required RenderFragment DropDownButton { get; set; }
    [Parameter]
    public required RenderFragment Options { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string IdDropdown { get; private set; } = null!;
    public string IdStart { get; private set; } = null!;
    public string IdButton { get; private set; } = null!;
    public string IdOptions { get; private set; } = null!;
    public string IdEnd { get; private set; } = null!;
    private void SetIDs(string id) {
        IdDropdown = id;
        IdStart = $"{IdStartPrefix}-{IdDropdown}";
        IdButton = $"{IdButtonPrefix}-{IdDropdown}";
        IdOptions = $"{IdOptionsPrefix}-{IdDropdown}";
        IdEnd = $"{IdEndPrefix}-{IdDropdown}";
    }
    // Ref:
    private readonly DotNetObjectReference<DropDown> Ref;
    // State:
    private bool OpenRequested = false;
    private bool Displayed { get; set; } = false;
    private readonly LockerSlim Lock = new();
    // Computed:
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(ClassDisplayed, Displayed);
    }
    private string ComputeLabel() => Displayed ? $"{I18N.T("Close")} {Title}" : $"{I18N.T("Open")} {Title}";

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------    
    public DropDown() {
        SetIDs(IDGenerator.Generate(IdPrefix));
        Ref = DotNetObjectReference.Create(this);
    }

    protected override async Task OnComponentInitializedAsync() {
        if (AppEnvironment.IsClient) {
            await Window.AddClickEventListener(Ref, JS_OnClick);
            await Window.AddScrollEventListener(Ref, JS_OnScroll);
            await Window.AddResizeEventListener(Ref, JS_OnResize);
            await Window.AddKeyDownEventListener(Ref, JS_OnKeyDown);
        }
    }

    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        if (Id != null) SetIDs(Id);
    }

    protected override async ValueTask OnComponentDisposeAsync() {
        if (AppEnvironment.IsClient) {
            await Window.RemoveClickEventListener(Ref, JS_OnClick);
            await Window.RemoveScrollEventListener(Ref, JS_OnScroll);
            await Window.RemoveResizeEventListener(Ref, JS_OnResize);
            await Window.RemoveKeyDownEventListener(Ref, JS_OnKeyDown);
        }
        Ref.Dispose();
        await Lock.DisposeSafe();
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
    public Func<Task> CreateAction(EventCallback action) => async () => {
        await Lock.TryExclusive(async () => {
            Displayed = false;
            SetFocus();
            StateHasChanged();
            await action.InvokeAsync();
        });
    };

    public void SetFocus() => ActionHandler.SetFocus(IdButton);

    // JS Interop -------------------------------------------------------------------------------------------------------------------------
    [JSInvokable]
    public async Task JS_OnClick(WindowMouseEvent e) {
        await Lock.TryExclusive(() => {
            Displayed = OpenRequested;
            OpenRequested = false;
            StateHasChanged();
        });
    }

    [JSInvokable]
    public async Task JS_OnScroll() => await Close();

    [JSInvokable]
    public async Task JS_OnResize(WindowResizeEvent e) => await Close();

    [JSInvokable]
    public async Task JS_OnKeyDown(WindowKeyEvent e) { if (e.Key == KeyBoard.Esc) await Close(); }
}
