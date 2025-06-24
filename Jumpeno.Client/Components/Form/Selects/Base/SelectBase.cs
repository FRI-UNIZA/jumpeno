namespace Jumpeno.Client.Components;

public partial class SelectBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "select";
    public const string CLASS_SELECT_BASE = "select-base";
    public const string CLASS_SELECT_INPUT = "select-input";
    public const string CLASS_SELECT_INPUT_EMPTY = "select-input-empty";
    public const string CLASS_SELECT_INPUT_TEXT = "select-input-text";
    public const string CLASS_SELECT_INPUT_ICON = "select-input-icon";
    public const string CLASS_OPTIONS = "options";
    public const string CLASS_OPTION = "option";
    public const string CLASS_OPTION_SELECTED = "option-selected";
    public static readonly SelectOption EMPTY_OPTION = new(null, I18N.T("Empty"));

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Styling:
    [Parameter]
    public string? MinWidth { get; set; } = null;
    [Parameter]
    public string? MaxWidth { get; set; } = null;
    [Parameter]
    public string? MinHeight { get; set; } = null;
    [Parameter]
    public string? MaxHeight { get; set; } = null;
    [Parameter]
    public SELECT_OPTION_ALIGN OptionAlign { get; set; } = SELECT_OPTION_ALIGN.LEFT;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private List<SelectOption> DisplayedOptions = [];
    private Modal ModalRef = null!;
    private SelectOption SelectedBefore { get; set; } = EMPTY_OPTION;
    private TaskCompletionSource SelectTCS = new();

    // CSS --------------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeWrapClass() => ComputeClass(CLASS_SELECT_BASE);
    protected CSSClass ComputeButtonClass() => ComputeClass(CLASS_SELECT_INPUT);
    private static CSSStyle ComputeModalStyle() => new("--modal-padding-content: 0");
    private CSSClass ComputeOptionClass(SelectOption option) {
        var c = new CSSClass(CLASS_OPTION);
        if (IsSelected(option)) c.Set(CLASS_OPTION_SELECTED);
        return c;
    }
    private CSSStyle ComputeOptionStyle() => new($"--option-align: {OptionAlign.StringLower()}");

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) {
        if (!firstTime) return;
        DisplayedOptions = [.. ViewModel.Options];
        ViewModel.SetValue(ViewModel.DefaultValue);
        if (ViewModel.Empty) DisplayedOptions.Insert(0, EMPTY_OPTION);
        else if (ViewModel.DefaultValue == EMPTY_OPTION) ViewModel.SetValue(ViewModel.Options[0]);
    }

    protected override void OnComponentAfterRender(bool firstRender) {
        if (!firstRender) SelectTCS.TrySetResult();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private bool IsSelected(SelectOption option) => ViewModel.Value == option;

    private async Task SelectOption(SelectOption option) {
        await PageLoader.Show(PAGE_LOADER_TASK.MODAL, true);
        if (SelectedBefore != option) {
            ViewModel.SetValue(option);
            SelectTCS = new TaskCompletionSource();
            StateHasChanged();
            await SelectTCS.Task;
            await ViewModel.OnSelect.Invoke(new SelectEvent(SelectedBefore, ViewModel.Value));
        }
        await ModalRef.Close();
        ActionHandler.SetFocus(ViewModel.FormID);
    }

    private async Task OpenModal() {
        await PageLoader.Show(PAGE_LOADER_TASK.SELECT, true);
        SelectedBefore = ViewModel.Value;
        await ModalRef.Open();
    }

    private async Task HandleBeforeOpen() {
        var pos = ModalRef.ScrollAreaRef.ItemPosition($".{CLASS_OPTION_SELECTED}");
        ModalRef.ScrollAreaRef.ScrollTo(0, pos.Top - pos.Height);
        await PageLoader.Hide(PAGE_LOADER_TASK.SELECT);
    }

    private async Task HandleAfterClose() {
        if (SelectedBefore != ViewModel.Value) {
            await ViewModel.OnCloseSelected.Invoke(new SelectEvent(SelectedBefore, ViewModel.Value));
        }
    }
}
