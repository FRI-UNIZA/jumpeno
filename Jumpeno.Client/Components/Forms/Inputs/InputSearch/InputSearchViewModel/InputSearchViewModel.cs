namespace Jumpeno.Client.ViewModels;

public class InputSearchViewModel : FormViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string DEFAULT_PLACEHOLDER => $"{I18N.T("Search")}...";

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    public readonly InputViewModel<string> InputVM = new(new InputViewModelTextParams());

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly INPUT_SEARCH_MODE SearchMode;
    public readonly bool Trim;
    public EventDelegate<string> OnSearch { get; set; }

    // Value ------------------------------------------------------------------------------------------------------------------------------
    private string LastSearchValue;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Search(string value) {
        if (Trim) value = value.Trim();
        InputVM.SetValue(value);
        switch (SearchMode) {
            case INPUT_SEARCH_MODE.LOWERCASE:
                value = value.ToLower();
            break;
            case INPUT_SEARCH_MODE.UPPERCASE:
                value = value.ToUpper();
            break;
        }
        if (value == LastSearchValue) return;
        LastSearchValue = value;
        await OnSearch.Invoke(value);
    }

    public void Clear() {
        InputVM.Clear();
        LastSearchValue = "";
        Notify();
    }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public InputSearchViewModel(InputSearchViewModelParams p) : base(p.Form, p.ID, p.OnError) {
        InputVM = new(new InputViewModelTextParams(
            Form: p.Form,
            ID: p.ID,
            TextMode: p.TextMode,
            Trim: false,
            TextCheck: p.TextCheck,
            MaxLength: p.MaxLength,
            Placeholder: p.Placeholder ?? DEFAULT_PLACEHOLDER,
            DefaultValue: p.DefaultValue,
            ClearValue: p.ClearValue,
            OnClear: new(async e => await Search(e.After)),
            OnEnter: new(async e => await Search(e.TextAfter))
        ));
        SearchMode = p.SearchMode;
        Trim = p.Trim;
        OnSearch = p.OnSearch ?? EventDelegate<string>.EMPTY;
        LastSearchValue = InputVM.Value;
    }
}
