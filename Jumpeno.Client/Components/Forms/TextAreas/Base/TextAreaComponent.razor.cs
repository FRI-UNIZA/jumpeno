namespace Jumpeno.Client.Components;
public partial class TextAreaComponent {
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public new const string Class = "textarea";
    public const string ClassContainer = "textarea-container";
    public const string ClassTextareaElement = "textarea-element";
    public const double DefaultMaxRows = 3.5;

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string Name { get; set; } = "";
    [Parameter]
    public bool AllowClear { get; set; } = false;
    [Parameter]
    public RenderFragment? Icon { get; set; } = null;
    [Parameter]
    public RenderFragment? IconAfter { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string TextAreaValue = "";
    private string FinalValue = "";
    private bool PendingResize = false;
    private readonly LockerSlim TextAreaLock = new();
    private bool ShouldPreventKeyDown = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    protected int ComputeTabindexClear() => ValueIsClear() ? -1 : 0;

    protected string ComputeTextAreaStyle() => ViewModel.AutoResize
        ? $"--textarea-max-rows: {DefaultMaxRows};"
        : string.Empty;

    public override CSSClass ComputeClass() {
        return base.ComputeClass()
        .Set(Class, Base)
        .Set($"text-mode-{ViewModel.TextMode.StringLower()}")
        .Set("allow-clear", AllowClear)
        .Set("value-default", ValueIsClear())
        .Set("auto-resize", ViewModel.AutoResize)
        .Set("icon-before", Icon is not null)
        .Set("icon-after", IconAfter is not null);
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        FormViewModel.SetReact(ViewModel, async () => {
            UpdateTextAreaValue();
            PendingResize = true;
            await InvokeAsync(StateHasChanged);
        });
        if (firstTime) UpdateTextAreaValue();
    }

    protected override void OnComponentDispose() => TextAreaLock.DisposeUnsafe();

    protected override void OnComponentAfterRender(bool firstRender) {
        base.OnComponentAfterRender(firstRender);
        if (!PendingResize) return;
        _ = InvokeAsync(async () => {
            PendingResize = false;
            await AutoResizeTextArea();
        });
    }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    private async Task InvokeUpdate(Action? setter = null) {
        var previousText = FinalValue;
        var previousValue = ViewModel.Value;
        if (setter != null) {
            setter();
        } else {
            ViewModel.Clear();
            ActionHandler.SetFocus(ViewModel.FormID);
            await ViewModel.OnClear.Invoke(new(previousText, FinalValue, previousValue, ViewModel.Value));
        }
        if (!ViewModel.Value.Equals(previousValue))
            await ViewModel.OnChange.Invoke(new(previousText, FinalValue, previousValue, ViewModel.Value));
        // No AutoResizeTextArea here — PendingResize + OnComponentAfterRender handles it
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private bool ValueIsClear() => FinalValue.Equals(ViewModel.ClearValue);

    private void UpdateTextAreaValue() { TextAreaValue = ViewModel.Value; FinalValue = TextAreaValue; }

    private async Task AutoResizeTextArea() {
        if (!ViewModel.AutoResize) return;
        try {
            await JSRuntime.InvokeVoidAsync("autoResizeTextArea", ViewModel.FormID);
        } catch { }
    }

    private async Task HandleInput(string? value) {
        await TextAreaLock.TryExclusive(async () => {
            var lastValue = FinalValue;
            try {
                if (value is null) return;
                if (ViewModel.MaxLength is not null && value.Length > ViewModel.MaxLength) return;
                if (ViewModel.TextCheck != null && !ViewModel.TextCheck(value)) return;
                TextAreaValue = value;
                FinalValue = ViewModel.ApplyTextMode(value);
            } catch {
                return;
            }
            ViewModel.Error.Clear();
            await ViewModel.OnInput.Invoke(new(lastValue, FinalValue, ViewModel.Value, ViewModel.Value));
        });
        // Resize after input — PendingResize not set here since value changed directly
        await AutoResizeTextArea();
    }

    private async Task HandleChange() {
        var val = FinalValue;
        string changedValue;
        try { changedValue = val; } catch { changedValue = ViewModel.ClearValue; }
        await InvokeUpdate(() => ViewModel.SetValue(changedValue));
    }

    private async Task HandleKeyDown(KeyboardEventArgs e) {
        if (e.Key == KEYBOARD.ENTER) {
            if (!e.ShiftKey) {
                // Plain Enter — fire OnEnter and prevent newline
                ShouldPreventKeyDown = true;
                await ViewModel.OnEnter.Invoke(new(FinalValue, FinalValue, ViewModel.Value, ViewModel.Value));
            } else {
                // Shift+Enter — insert newline manually, prevent double newline from browser
                ShouldPreventKeyDown = true;
                await HandleInput(FinalValue + "\n");
            }
        } else {
            ShouldPreventKeyDown = false;
        }
    }
}
