namespace Jumpeno.Client.Components;

public partial class SelectMultiComponent<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string IdPrefix = "select-multi";
    // Class:
    public new const string ClassName = "select-multi";
    public const string ClassSelectInput = "select-multi-input";
    public const string ClassSelectInputEmpty = "select-multi-input-empty";
    public const string ClassSelectInputPlaceholder = "select-multi-input-placeholder";
    public const string ClassSelectInputText = "select-multi-input-text";
    public const string ClassSelectInputIndicators = "select-multi-input-indicators";
    public const string ClassSelectInputCount = "select-multi-input-count";
    public const string ClassSelectInputPlus = "select-multi-input-plus";
    public const string ClassSelectInputIcon = "select-multi-input-icon";
    public const string ClassSelectOptionsModal = "select-multi-options-modal";
    public const string ClassSelectInputSearch = "select-multi-input-search";
    public const string ClassOptions = "select-multi-options";
    public const string ClassOption = "select-multi-option";
    public const string ClassOptionSelected = "select-multi-option-selected";
    public const string ClassOptionMarker = "select-multi-option-marker";
    public const string ClassOptionMarkerDisplayed = "select-multi-option-marker-displayed";
    public const string ClassSelectEmptyText = "select-multi-empty-text";
    // Search:
    private const int MinSearchLoading = 175; // ms
    // Close:
    private enum SelectMultiClose { Cancel, Clear, Ok }

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
    private Dictionary<string, SelectOption<T>> DisplayedValue = [];
    private SelectMultiClose ClosedAs = SelectMultiClose.Cancel;
    private bool ValueChanged = false;
    private Dictionary<string, SelectOption<T>> LastValue = [];
    // Tasks:
    private TaskCompletionSource _searchTcs = new();
    private readonly MinWatch MinSearchTime = new(MinSearchLoading);

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private string OptionPlaceholder() {
        if (ViewModel.Value.Count == 0) {
            if (ViewModel.Placeholder != null) return ViewModel.Placeholder;
            return I18N.T("Empty");
        }
        var firstValue = ViewModel.Value.Values.FirstOrDefault();
        if (ViewModel.Value.Count == 1 && firstValue != null)
            return I18N.T("Selected: I18N{option}", new() {{"option", firstValue.Label }});
        else if (firstValue != null)
            return I18N.T("Selected: I18N{option}, plus: I18N{count}", new() {{"option", firstValue.Label}, {"count", ViewModel.Value.Count - 1 }});
        else
            return I18N.T("Selected total: I18N{count}", new() {{"count", ViewModel.Value.Count}});
    }

    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);

    private CssClass ComputeModalClass() => new CssClass(ClassSelectOptionsModal).Set(ModalClass).Set(OptionAlign);

    private CssClass ComputeOptionClass(SelectOption<T> option) {
        var c = new CssClass(ClassOption);
        if (DisplayedValue.ContainsKey(option.Label)) c.Set(ClassOptionSelected);
        return c;
    }

    private CssClass ComputeMarkerClass(bool isSelected, bool isPlus) {
        var c = new CssClass(ClassOptionMarker);
        if (isPlus && !isSelected) c.Set(ClassOptionMarkerDisplayed);
        else if (!isPlus && isSelected) c.Set(ClassOptionMarkerDisplayed);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) => ViewModel.SearchVM.OnSearch = new(Search);

    protected override void OnComponentAfterRender(bool firstRender) {
        if (firstRender) return;
        _searchTcs.TrySetResult();
    }

    // Opening ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() {
        if (Disabled) return;
        await ModalRef.Open();
    }

    private void HandleOpenStart() {
        ViewModel.SearchVM.Clear();
        DisplayedOptions = [..ViewModel.Options];
        DisplayedValue = new(ViewModel.Value);
        ClosedAs = SelectMultiClose.Cancel;
        ValueChanged = false;
        LastValue = [];
    }

    // Search -----------------------------------------------------------------------------------------------------------------------------
    private Task Search(string value) => UI.Lock.TryExclusive(async () => {
        await PageLoader.Show(PageLoaderTask.Search);
        MinSearchTime.Start();
        List<SelectOption<T>> newOptions = [];
        foreach (var option in ViewModel.Options) {
            if (ViewModel.CustomSearch(new(value, option))) {
                newOptions.Add(option);
            }
        }
        _searchTcs = new();
        DisplayedOptions = newOptions;
        StateHasChanged();
        await _searchTcs.Task;
        await MinSearchTime.Task;
        await PageLoader.Hide(PageLoaderTask.Search);
    });

    // Select -----------------------------------------------------------------------------------------------------------------------------
    private Task SelectOption(SelectOption<T> option, bool isSelected) => UI.Lock.TryExclusive(async () => {
        if (isSelected) {
            DisplayedValue.Remove(option.Label);
            await ViewModel.OnDeselect.Invoke(new(option));
        } else {
            DisplayedValue.Add(option.Label, option);
            await ViewModel.OnSelect.Invoke(new(option));
        }
    });

    private Task ClearSelect() => ModalRef.Close(async () => {
        await PageLoader.Show(PageLoaderTask.Modal, true);
        ClosedAs = SelectMultiClose.Clear;
        LastValue = new(ViewModel.Value);
        ValueChanged = ViewModel.SetValue(new Dictionary<string, SelectOption<T>>());
    });

    private Task ConfirmSelect() => ModalRef.Close(async () => {
        await PageLoader.Show(PageLoaderTask.Modal, true);
        ClosedAs = SelectMultiClose.Ok;
        LastValue = new(ViewModel.Value);
        ValueChanged = ViewModel.SetValue(DisplayedValue);
    });

    // Close ------------------------------------------------------------------------------------------------------------------------------
    public Task Close() => ModalRef.Close();

    private async Task HandleCloseStart() {
        switch (ClosedAs) {
            case SelectMultiClose.Cancel:
                await ViewModel.OnCancel.Invoke(new(DisplayedValue, ViewModel.Value));
            break;
            case SelectMultiClose.Clear:
                if (ValueChanged) await ViewModel.OnClear.Invoke(new(LastValue, ViewModel.Value));
                else await ViewModel.OnCancel.Invoke(new(DisplayedValue, ViewModel.Value));
            break;
            case SelectMultiClose.Ok:
                if (ValueChanged) await ViewModel.OnOK.Invoke(new(LastValue, ViewModel.Value));
                else await ViewModel.OnCancel.Invoke(new(DisplayedValue, ViewModel.Value));
            break;
        }
    }

    private async Task HandleCloseFinish() {
        switch (ClosedAs) {
            case SelectMultiClose.Cancel:
                await ViewModel.OnCancelClose.Invoke(new(DisplayedValue, ViewModel.Value));
            break;
            case SelectMultiClose.Clear:
                if (ValueChanged) await ViewModel.OnClearClose.Invoke(new(LastValue, ViewModel.Value));
                else await ViewModel.OnCancelClose.Invoke(new(DisplayedValue, ViewModel.Value));
            break;
            case SelectMultiClose.Ok:
                if (ValueChanged) await ViewModel.OnOKClose.Invoke(new(LastValue, ViewModel.Value));
                else await ViewModel.OnCancelClose.Invoke(new(DisplayedValue, ViewModel.Value));
            break;
        }
    }

    private async Task HandleAfterCloseFinish() {
        DisplayedOptions = [];
        var displayedValue = DisplayedValue; DisplayedValue = [];
        var lastValue = LastValue; LastValue = [];
        switch (ClosedAs) {
            case SelectMultiClose.Cancel:
                await ViewModel.OnAfterCancelClose.Invoke(new(displayedValue, ViewModel.Value));
            break;
            case SelectMultiClose.Clear:
                if (ValueChanged) await ViewModel.OnAfterClearClose.Invoke(new(lastValue, ViewModel.Value));
                else await ViewModel.OnAfterCancelClose.Invoke(new(displayedValue, ViewModel.Value));
            break;
            case SelectMultiClose.Ok:
                if (ValueChanged) await ViewModel.OnAfterOKClose.Invoke(new(lastValue, ViewModel.Value));
                else await ViewModel.OnAfterCancelClose.Invoke(new(displayedValue, ViewModel.Value));
            break;
        }
    }
}
