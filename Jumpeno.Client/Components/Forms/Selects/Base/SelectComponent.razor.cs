namespace Jumpeno.Client.Components;

public partial class SelectComponent<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IdPrefix = "select";
    // Class:
    public new const string ClassName = "select";
    public const string ClassSelectInput = "select-input";
    public const string ClassSelectInputEmpty = "select-input-empty";
    public const string ClassSelectInputPlaceholder = "select-input-placeholder";
    public const string ClassSelectInputText = "select-input-text";
    public const string ClassSelectInputIcon = "select-input-icon";
    public const string ClassSelectOptionsModal = "select-options-modal";
    public const string ClassSelectInputSearch = "select-input-search";
    public const string ClassOptions = "select-options";
    public const string ClassOption = "select-option";
    public const string ClassOptionSelected = "select-option-selected";
    public const string ClassSelectEmptyText = "select-empty-text";
    // Search:
    private const int MinSearchLoading = 175; // ms

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    // Modal:
    [Parameter]
    public string ModalClass { get; set; } = "";
    [Parameter]
    public ModalSurface? MSurface { get; set; } = ModalSurface.Floating;
    // Search:
    [Parameter]
    public FormSize? SearchSize { get; set; } = FormSize.S;
    [Parameter]
    public FormAlign? SearchAlign { get; set; } = FormAlign.Left;
    // Options:
    [Parameter]
    public SelectOptionAlign? OptionAlign { get; set; } = SelectOptionAlign.Left;

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private Modal ModalRef = null!;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Options:
    private List<SelectOption<T>> DisplayedOptions = [];
    private SelectOption<T> LastSelected { get; set; } = Select<T>.EmptyOption;
    // Tasks:
    private TaskCompletionSource _searchTcs = new();
    private TaskCompletionSource _selectTcs = new();
    private readonly MinWatch MinSearchWatch = new(MinSearchLoading);

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private string OptionPlaceholder() {
        if (ViewModel.Value == Select<T>.EmptyOption) {
            if (ViewModel.Placeholder != null) return ViewModel.Placeholder;
            return I18N.T("Empty");
        }
        return I18N.T("Selected: I18N{option}", new() {{"option", ViewModel.Value.Label }});
    }

    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);

    private CssClass ComputeModalClass() => new CssClass(ClassSelectOptionsModal).Set(ModalClass).Set(OptionAlign);

    private CssClass ComputeOptionClass(SelectOption<T> option) {
        var c = new CssClass(ClassOption);
        c.Set(ClassOptionSelected, ViewModel.Value == option);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) => ViewModel.SearchVM.OnSearch = new(Search);

    protected override void OnComponentAfterRender(bool firstRender) {
        if (firstRender) return;
        _searchTcs.TrySetResult();
        _selectTcs.TrySetResult();
    }

    // Opening ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        if (Disabled) return;
        await ModalRef.Open();
    }

    private void HandleOpenStart() {
        ViewModel.SearchVM.Clear();
        DisplayedOptions = [.. ViewModel.Options];
        if (ViewModel.Empty) DisplayedOptions.Insert(0, Select<T>.EmptyOption);
        LastSelected = ViewModel.Value;
    }

    private void HandleOpenFinish() {
        var pos = ModalRef.ScrollAreaRef.ItemPosition($".{ClassOptionSelected}");
        ModalRef.ScrollAreaRef.InitScrollTo(0, pos.Top - pos.Height);
    }

    // Search -----------------------------------------------------------------------------------------------------------------------------
    private Task Search(string value) => UI.Lock.TryExclusive(async () => {
        await PageLoader.Show(PageLoaderTask.Search);
        MinSearchWatch.Start();
        List<SelectOption<T>> newOptions = [];
        if (ViewModel.Empty && value == ViewModel.SearchVM.InputVM.ClearValue) newOptions.Add(Select<T>.EmptyOption);
        foreach (var option in ViewModel.Options) {
            if (ViewModel.CustomSearch(new(value, option))) {
                newOptions.Add(option);
            }
        }
        _searchTcs = new();
        DisplayedOptions = newOptions;
        StateHasChanged();
        await _searchTcs.Task;
        await MinSearchWatch.Task;
        await PageLoader.Hide(PageLoaderTask.Search);
    });

    // Select -----------------------------------------------------------------------------------------------------------------------------
    private Task SelectOption(SelectOption<T> option) => ModalRef.Close(async () => {
        if (LastSelected == option) return;
        await PageLoader.Show(PageLoaderTask.Modal, true);
        ViewModel.SetValue(option);
        _selectTcs = new TaskCompletionSource();
        StateHasChanged();
        await _selectTcs.Task;
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
        var lastSelected = LastSelected; LastSelected = Select<T>.EmptyOption;
        if (lastSelected == ViewModel.Value) return;
        await ViewModel.OnAfterCloseSelected.Invoke(new SelectEvent<T>(lastSelected, ViewModel.Value));
    }
}
