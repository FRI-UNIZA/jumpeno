namespace Jumpeno.Client.Components;

public partial class SelectComponent {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "select";
    // Class:
    public new const string CLASS = "select";
    public const string CLASS_SELECT_INPUT = "select-input";
    public const string CLASS_SELECT_INPUT_EMPTY = "select-input-empty";
    public const string CLASS_SELECT_INPUT_PLACEHOLDER = "select-input-placeholder";
    public const string CLASS_SELECT_INPUT_TEXT = "select-input-text";
    public const string CLASS_SELECT_INPUT_ICON = "select-input-icon";
    public const string CLASS_SELECT_OPTIONS_MODAL = "select-options-modal";
    public const string CLASS_SELECT_INPUT_SEARCH = "select-input-search";
    public const string CLASS_OPTIONS = "select-options";
    public const string CLASS_OPTION = "select-option";
    public const string CLASS_OPTION_SELECTED = "select-option-selected";
    public const string CLASS_SELECT_EMPTY_TEXT = "select-empty-text";
    // Search:
    private const int MIN_SEARCH_LOADING = 175; // ms

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Modal:
    [Parameter]
    public string ModalClass { get; set; } = "";
    [Parameter]
    public MODAL_SURFACE? ModalSurface { get; set; } = MODAL_SURFACE.FLOATING;
    // Search:
    [Parameter]
    public FORM_SIZE? SearchSize { get; set; } = FORM_SIZE.S;
    [Parameter]
    public FORM_ALIGN? SearchAlign { get; set; } = FORM_ALIGN.LEFT;
    // Options:
    [Parameter]
    public SELECT_OPTION_ALIGN? OptionAlign { get; set; } = SELECT_OPTION_ALIGN.LEFT;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Options:
    private List<SelectOption> DisplayedOptions = [];
    private SelectOption LastSelected { get; set; } = SELECT.EMPTY_OPTION;
    // Tasks:
    private TaskCompletionSource SearchTCS = new();
    private TaskCompletionSource SelectTCS = new();
    private readonly MinWatch MinSearchWatch = new(MIN_SEARCH_LOADING);

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private string OptionPlaceholder() {
        if (ViewModel.Value == SELECT.EMPTY_OPTION) {
            if (ViewModel.Placeholder != null) return ViewModel.Placeholder;
            return I18N.T("Empty");
        }
        return I18N.T("Selected: I18N{option}", new() {{"option", ViewModel.Value.Label }});
    }

    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    private CSSClass ComputeModalClass() => new CSSClass(CLASS_SELECT_OPTIONS_MODAL).Set(ModalClass).Set(OptionAlign);

    private CSSClass ComputeOptionClass(SelectOption option) {
        var c = new CSSClass(CLASS_OPTION);
        if (ViewModel.Value == option) c.Set(CLASS_OPTION_SELECTED);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) => ViewModel.SearchVM.OnSearch = new(Search);

    protected override void OnComponentAfterRender(bool firstRender) {
        if (firstRender) return;
        SearchTCS.TrySetResult();
        SelectTCS.TrySetResult();
    }

    // Opening ----------------------------------------------------------------------------------------------------------------------------
    private async Task OpenModal() {
        if (Disabled) return;
        await PageLoader.Show(PAGE_LOADER_TASK.SELECT, true);
        ViewModel.SearchVM.Clear();
        DisplayedOptions = [.. ViewModel.Options];
        if (ViewModel.Empty) DisplayedOptions.Insert(0, SELECT.EMPTY_OPTION);
        LastSelected = ViewModel.Value;
        await ModalRef.Open();
    }

    private async Task HandleBeforeOpen() {
        var pos = ModalRef.ScrollAreaRef.ItemPosition($".{CLASS_OPTION_SELECTED}");
        ModalRef.ScrollAreaRef.ScrollTo(0, pos.Top - pos.Height);
        await PageLoader.Hide(PAGE_LOADER_TASK.SELECT);
    }

    // Search -----------------------------------------------------------------------------------------------------------------------------
    private async Task Search(string value) {
        await PageLoader.Show(PAGE_LOADER_TASK.SEARCH);
        MinSearchWatch.Start();
        List<SelectOption> newOptions = [];
        if (ViewModel.Empty && value == ViewModel.SearchVM.InputVM.ClearValue) newOptions.Add(SELECT.EMPTY_OPTION);
        foreach (var option in ViewModel.Options) {
            if (ViewModel.CustomSearch(new(value, option))) {
                newOptions.Add(option);
            }
        }
        SearchTCS = new();
        DisplayedOptions = newOptions;
        StateHasChanged();
        await SearchTCS.Task;
        await MinSearchWatch.Task;
        await PageLoader.Hide(PAGE_LOADER_TASK.SEARCH);
    }

    // Select -----------------------------------------------------------------------------------------------------------------------------
    private async Task SelectOption(SelectOption option) {
        await PageLoader.Show(PAGE_LOADER_TASK.MODAL, true);
        if (LastSelected != option) {
            ViewModel.SetValue(option);
            SelectTCS = new TaskCompletionSource();
            StateHasChanged();
            await SelectTCS.Task;
            await ViewModel.OnSelect.Invoke(new SelectEvent(LastSelected, ViewModel.Value));
        }
        await ModalRef.Close();
        ActionHandler.SetFocus(ViewModel.FormID);
    }

    // Close ------------------------------------------------------------------------------------------------------------------------------
    private async Task HandleAfterClose() {
        DisplayedOptions = [];
        var lastSelected = LastSelected; LastSelected = SELECT.EMPTY_OPTION;
        if (lastSelected != ViewModel.Value) {
            await ViewModel.OnCloseSelected.Invoke(new SelectEvent(lastSelected, ViewModel.Value));
        }
    }
}
