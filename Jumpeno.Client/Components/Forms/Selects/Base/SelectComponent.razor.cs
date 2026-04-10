namespace Jumpeno.Client.Components;

public partial class SelectComponent<T> {
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
    public ModalSurface? MSurface { get; set; } = ModalSurface.FLOATING;
    // Search:
    [Parameter]
    public FormSize? SearchSize { get; set; } = FormSize.S;
    [Parameter]
    public FormAlign? SearchAlign { get; set; } = FormAlign.LEFT;
    // Options:
    [Parameter]
    public SelectOptionAlign? OptionAlign { get; set; } = SelectOptionAlign.LEFT;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Options:
    private List<SelectOption<T>> DisplayedOptions = [];
    private SelectOption<T> LastSelected { get; set; } = Select<T>.EMPTY_OPTION;
    // Tasks:
    private TaskCompletionSource SearchTCS = new();
    private TaskCompletionSource SelectTCS = new();
    private readonly MinWatch MinSearchWatch = new(MIN_SEARCH_LOADING);

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private string OptionPlaceholder() {
        if (ViewModel.Value == Select<T>.EMPTY_OPTION) {
            if (ViewModel.Placeholder != null) return ViewModel.Placeholder;
            return I18N.T("Empty");
        }
        return I18N.T("Selected: I18N{option}", new() {{"option", ViewModel.Value.Label }});
    }

    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    private CSSClass ComputeModalClass() => new CSSClass(CLASS_SELECT_OPTIONS_MODAL).Set(ModalClass).Set(OptionAlign);

    private CSSClass ComputeOptionClass(SelectOption<T> option) {
        var c = new CSSClass(CLASS_OPTION);
        c.Set(CLASS_OPTION_SELECTED, ViewModel.Value == option);
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
    public async Task Open() {
        if (Disabled) return;
        await ModalRef.Open();
    }

    private void HandleOpenStart() {
        ViewModel.SearchVM.Clear();
        DisplayedOptions = [.. ViewModel.Options];
        if (ViewModel.Empty) DisplayedOptions.Insert(0, Select<T>.EMPTY_OPTION);
        LastSelected = ViewModel.Value;
    }

    private void HandleOpenFinish() {
        var pos = ModalRef.ScrollAreaRef.ItemPosition($".{CLASS_OPTION_SELECTED}");
        ModalRef.ScrollAreaRef.InitScrollTo(0, pos.Top - pos.Height);
    }

    // Search -----------------------------------------------------------------------------------------------------------------------------
    private Task Search(string value) => UI.Lock.TryExclusive(async () => {
        await PageLoader.Show(PageLoaderTask.SEARCH);
        MinSearchWatch.Start();
        List<SelectOption<T>> newOptions = [];
        if (ViewModel.Empty && value == ViewModel.SearchVM.InputVM.ClearValue) newOptions.Add(Select<T>.EMPTY_OPTION);
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
        await PageLoader.Hide(PageLoaderTask.SEARCH);
    });

    // Select -----------------------------------------------------------------------------------------------------------------------------
    private Task SelectOption(SelectOption<T> option) => ModalRef.Close(async () => {
        if (LastSelected == option) return;
        await PageLoader.Show(PageLoaderTask.MODAL, true);
        ViewModel.SetValue(option);
        SelectTCS = new TaskCompletionSource();
        StateHasChanged();
        await SelectTCS.Task;
        await ViewModel.OnSelect.Invoke(new SelectEvent<T>(LastSelected, ViewModel.Value));
    });

    // Close ------------------------------------------------------------------------------------------------------------------------------
    public Task Close() => ModalRef.Close();

    private async Task HandleCloseFinish() {
        if (LastSelected == ViewModel.Value) return;
        await ViewModel.OnCloseSelected.Invoke(new SelectEvent<T>(LastSelected, ViewModel.Value));
    }

    private async Task HandleAfterCloseFinish() {
        ActionHandler.SetFocus(ViewModel.FormID);
        DisplayedOptions = [];
        var lastSelected = LastSelected; LastSelected = Select<T>.EMPTY_OPTION;
        if (lastSelected == ViewModel.Value) return;
        await ViewModel.OnAfterCloseSelected.Invoke(new SelectEvent<T>(lastSelected, ViewModel.Value));
    }
}
