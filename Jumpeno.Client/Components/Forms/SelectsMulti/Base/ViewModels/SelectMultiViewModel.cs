namespace Jumpeno.Client.ViewModels;

public class SelectMultiViewModel : FormViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public readonly Predicate<SelectSearchEvent> DEFAULT_CUSTOM_SEARCH = e => e.Option.Label.ToLower().IndexOf(e.Search) >= 0;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Options:
    public readonly List<SelectOption> Options;
    public readonly List<SelectOption> DefaultValue;
    public readonly string? Placeholder;
    // Search:
    public readonly bool Search;
    public readonly Predicate<SelectSearchEvent> CustomSearch;
    // Search input:
    public readonly InputSearchViewModel SearchVM;
    // Value:
    public Dictionary<string, SelectOption> Value { get; private set; }
    public bool SetValue(List<SelectOption> list) {
        if (IsValueEqual(list)) return false;
        Value = list.ToDictionary(option => option.Label, option => option);
        Error.Clear();
        return true;
    }
    public bool SetValue(Dictionary<string, SelectOption> value) {
        if (IsValueEqual(value)) return false;
        Value = new(value);
        Error.Clear();
        return true;
    }
    public bool SelectOption(SelectOption option) {
        if (!Value.TryAdd(option.Label, option)) return false;
        Error.Clear();
        return true;
    }
    public bool DeselectOption(SelectOption option) {
        if (!Value.Remove(option.Label)) return false;
        Error.Clear();
        return true;
    }
    // Predicates:
    public bool IsValueEqual(IEnumerable<string> keys) => keys.Count() == Value.Count && keys.All(k => Value.ContainsKey(k));
    public bool IsValueEqual(List<SelectOption> list) => list.Count == Value.Count && list.All(o => Value.ContainsKey(o.Label));
    public bool IsValueEqual(Dictionary<string, SelectOption> value) => IsValueEqual(value.Keys);

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<SelectMultiOptionEvent> OnSelect { get; set; }
    public EventDelegate<SelectMultiOptionEvent> OnDeselect { get; set; }
    public EventDelegate<SelectMultiCancelEvent> OnCancel { get; set; }
    public EventDelegate<SelectMultiCancelEvent> OnCancelClose { get; set; }
    public EventDelegate<SelectMultiEvent> OnClear { get; set; }
    public EventDelegate<SelectMultiEvent> OnClearClose { get; set; }
    public EventDelegate<SelectMultiEvent> OnOK { get; set; }
    public EventDelegate<SelectMultiEvent> OnOKClose { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SelectMultiViewModel(SelectMultiViewModelParams p) : base(p.Form, p.ID, p.OnError) {
        if (p.Options == null) {
            throw new InvalidDataException("Empty select options!");
        }
        // Options:
        Options = p.Options ?? [];
        DefaultValue = p.DefaultValue ?? [];
        Placeholder = p.Placeholder;
        // Search:
        Search = p.Search;
        CustomSearch = p.CustomSearch ?? DEFAULT_CUSTOM_SEARCH;
        // Search input:
        SearchVM = new(new(
            TextMode: p.SearchTextMode,
            SearchMode: p.SearchMode,
            Trim: p.SearchTrim,
            TextCheck: p.SearchTextCheck,
            MaxLength: p.SearchMaxLength
        ));
        // Value:
        Value = DefaultValue.ToDictionary(option => option.Label, option => option);
        // Events:
        OnSelect = p.OnSelect ?? EventDelegate<SelectMultiOptionEvent>.EMPTY;
        OnDeselect = p.OnDeselect ?? EventDelegate<SelectMultiOptionEvent>.EMPTY;
        OnCancel = p.OnCancel ?? EventDelegate<SelectMultiCancelEvent>.EMPTY;
        OnCancelClose = p.OnCancelClose ?? EventDelegate<SelectMultiCancelEvent>.EMPTY;
        OnClear = p.OnClear ?? EventDelegate<SelectMultiEvent>.EMPTY;
        OnClearClose = p.OnClearClose ?? EventDelegate<SelectMultiEvent>.EMPTY;
        OnOK = p.OnOK ?? EventDelegate<SelectMultiEvent>.EMPTY;
        OnOKClose = p.OnOKClose ?? EventDelegate<SelectMultiEvent>.EMPTY;
    }
}
