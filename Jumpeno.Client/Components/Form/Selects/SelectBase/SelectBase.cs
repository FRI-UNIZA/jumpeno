namespace Jumpeno.Client.Components;

public partial class SelectBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "select";
    public const string CLASS_SELECT_INPUT_WRAP = "select-input-wrap";
    public const string CLASS_SELECT_INPUT = "select-input";
    public const string CLASS_SELECT_INPUT_EMPTY = "select-input-empty";
    public const string CLASS_SELECT_INPUT_TEXT = "select-input-text";
    public const string CLASS_SELECT_INPUT_ICON = "select-input-icon";
    public const string CLASS_OPTIONS = "options";
    public const string CLASS_OPTION = "option";
    public const string CLASS_OPTION_SELECTED = "option-selected";
    public static readonly SelectOption EMPTY_OPTION = new(null, I18N.T("Empty"));

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string ID { get; set; } = "";

    // Content:
    [Parameter]
    public required string Label { get; set; }
    [Parameter]
    public bool HideLabel { get; set; } = false;
    [Parameter]
    public required List<SelectOption> Options { get; set; }
    [Parameter]
    public SelectOption DefaultValue { get; set; } = EMPTY_OPTION;
    
    // Abilities:
    [Parameter]
    public bool Empty { get; set; } = false;

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

    // Callbacks:
    [Parameter]
    public EventCallback<SelectEvent> OnSelect { get; set; } = EventCallback<SelectEvent>.Empty;
    [Parameter]
    public EventCallback<SelectEvent> OnCloseSelected { get; set; } = EventCallback<SelectEvent>.Empty;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private List<SelectOption> DisplayedOptions = [];
    private Modal ModalRef = null!;
    public SelectOption Selected { get; private set; } = EMPTY_OPTION;
    public SelectOption SelectedBefore { get; private set; } = EMPTY_OPTION;
    private TaskCompletionSource SelectTCS = new();

    protected CSSClass ComputeWrapperClass() {
        return ComputeClass(CLASS_SELECT_INPUT_WRAP);
    }
    protected CSSClass ComputeButtonClass() {
        return ComputeClass(CLASS_SELECT_INPUT);
    }
    private static CSSStyle ComputeModalStyle() {
        return new CSSStyle("--modal-padding-content: 0");
    }
    private CSSClass ComputeOptionClass(SelectOption option) {
        var c = new CSSClass(CLASS_OPTION);
        if (IsSelected(option)) c.Set(CLASS_OPTION_SELECTED);
        return c;
    }
    private CSSStyle ComputeOptionStyle() {
        return new CSSStyle($"--option-align: {OptionAlign.StringLower()}");
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnParametersSet(bool firstTime) {
        if (!firstTime) return;
        if (ID == "") ID = ComponentService.GenerateID(ID_PREFIX);
        if (Options.Count < 1) throw new Exception("Select list can not be empty!");
        DisplayedOptions = new List<SelectOption>(Options);
        Selected = DefaultValue;
        if (Empty) DisplayedOptions.Insert(0, EMPTY_OPTION);
        else if (DefaultValue == EMPTY_OPTION) Selected = Options[0];
    }

    protected override void OnAfterRender(bool firstRender) {
        if (firstRender) return;
        SelectTCS.TrySetResult();
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private bool IsSelected(SelectOption option) {
        return Selected == option;
    }

    private async Task SelectOption(SelectOption option) {
        await PageLoader.Show(PAGE_LOADER_TASK.MODAL, true);
        if (SelectedBefore != option) {
            Selected = option;
            SelectTCS = new TaskCompletionSource();
            StateHasChanged();
            await SelectTCS.Task;
            await OnSelect.InvokeAsync(new SelectEvent(SelectedBefore, Selected));
        }
        await ModalRef.Close();
        ActionHandler.SetFocus(ID);
    }

    private async Task OpenModal() {
        await PageLoader.Show(PAGE_LOADER_TASK.SELECT, true);
        SelectedBefore = Selected;
        await ModalRef.Open();
    }

    private async Task HandleBeforeOpen() {
        var pos = ModalRef.ScrollAreaRef.ItemPosition($".{CLASS_OPTION_SELECTED}");
        ModalRef.ScrollAreaRef.ScrollTo(0, pos.Top - pos.Height);
        await PageLoader.Hide(PAGE_LOADER_TASK.SELECT);
    }

    private async Task HandleAfterClose() {
        if (SelectedBefore != Selected) {
            await OnCloseSelected.InvokeAsync(new SelectEvent(SelectedBefore, Selected));
        }
    }
}
