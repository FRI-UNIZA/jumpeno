namespace Jumpeno.Client.Components;

public partial class SelectMultiComponent<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID_PREFIX = "select-multi";
    // Class:
    public new const string CLASS = "select-multi";
    public const string CLASS_SELECT_INPUT = "select-multi-input";
    public const string CLASS_SELECT_INPUT_EMPTY = "select-multi-input-empty";
    public const string CLASS_SELECT_INPUT_PLACEHOLDER = "select-multi-input-placeholder";
    public const string CLASS_SELECT_INPUT_TEXT = "select-multi-input-text";
    public const string CLASS_SELECT_INPUT_INDICATORS = "select-multi-input-indicators";
    public const string CLASS_SELECT_INPUT_COUNT = "select-multi-input-count";
    public const string CLASS_SELECT_INPUT_PLUS = "select-multi-input-plus";
    public const string CLASS_SELECT_INPUT_ICON = "select-multi-input-icon";
    public const string CLASS_SELECT_OPTIONS_MODAL = "select-multi-options-modal";
    public const string CLASS_SELECT_INPUT_SEARCH = "select-multi-input-search";
    public const string CLASS_OPTIONS = "select-multi-options";
    public const string CLASS_OPTION = "select-multi-option";
    public const string CLASS_OPTION_SELECTED = "select-multi-option-selected";
    public const string CLASS_OPTION_MARKER = "select-multi-option-marker";
    public const string CLASS_OPTION_MARKER_DISPLAYED = "select-multi-option-marker-displayed";
    public const string CLASS_SELECT_EMPTY_TEXT = "select-multi-empty-text";
    // Search:
    private const int MIN_SEARCH_LOADING = 175; // ms
    // Close:
    private enum SelectMultiClose { Cancel, Clear, OK }

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
    private TaskCompletionSource SearchTCS = new();
    private readonly MinWatch MinSearchTime = new(MIN_SEARCH_LOADING);

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

    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    private CSSClass ComputeModalClass() => new CSSClass(CLASS_SELECT_OPTIONS_MODAL).Set(ModalClass).Set(OptionAlign);

    private CSSClass ComputeOptionClass(SelectOption<T> option) {
        var c = new CSSClass(CLASS_OPTION);
        if (DisplayedValue.ContainsKey(option.Label)) c.Set(CLASS_OPTION_SELECTED);
        return c;
    }

    private CSSClass ComputeMarkerClass(bool isSelected, bool isPlus) {
        var c = new CSSClass(CLASS_OPTION_MARKER);
        if (isPlus && !isSelected) c.Set(CLASS_OPTION_MARKER_DISPLAYED);
        else if (!isPlus && isSelected) c.Set(CLASS_OPTION_MARKER_DISPLAYED);
        return c;
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnComponentParametersSet(bool firstTime) => ViewModel.SearchVM.OnSearch = new(Search);

    protected override void OnComponentAfterRender(bool firstRender) {
        if (firstRender) return;
        SearchTCS.TrySetResult();
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
        SearchTCS = new();
        DisplayedOptions = newOptions;
        StateHasChanged();
        await SearchTCS.Task;
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
        ClosedAs = SelectMultiClose.OK;
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
            case SelectMultiClose.OK:
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
            case SelectMultiClose.OK:
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
            case SelectMultiClose.OK:
                if (ValueChanged) await ViewModel.OnAfterOKClose.Invoke(new(lastValue, ViewModel.Value));
                else await ViewModel.OnAfterCancelClose.Invoke(new(displayedValue, ViewModel.Value));
            break;
        }
    }
}
