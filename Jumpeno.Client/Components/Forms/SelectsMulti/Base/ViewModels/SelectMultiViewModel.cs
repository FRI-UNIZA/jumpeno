namespace Jumpeno.Client.ViewModels;

public class SelectMultiViewModel<T> : FormViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public readonly Predicate<SelectSearchEvent<T>> DEFAULT_CUSTOM_SEARCH = e => e.Option.Label.ToLower().IndexOf(e.Search) >= 0;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Options:
    public readonly List<SelectOption<T>> Options;
    public readonly List<SelectOption<T>> DefaultValue;
    public readonly string? Placeholder;
    // Search:
    public readonly bool Search;
    public readonly Predicate<SelectSearchEvent<T>> CustomSearch;
    // Search input:
    public readonly InputSearchViewModel SearchVM;
    // Value:
    public Dictionary<string, SelectOption<T>> Value { get; private set; }
    public bool SetValue(List<SelectOption<T>> list) {
        if (IsValueEqual(list)) return false;
        Value = list.ToDictionary(option => option.Label, option => option);
        Error.Clear();
        return true;
    }
    public bool SetValue(Dictionary<string, SelectOption<T>> value) {
        if (IsValueEqual(value)) return false;
        Value = new(value);
        Error.Clear();
        return true;
    }
    public bool SelectOption(SelectOption<T> option) {
        if (!Value.TryAdd(option.Label, option)) return false;
        Error.Clear();
        return true;
    }
    public bool DeselectOption(SelectOption<T> option) {
        if (!Value.Remove(option.Label)) return false;
        Error.Clear();
        return true;
    }
    // Predicates:
    public bool IsValueEqual(IEnumerable<string> keys) => keys.Count() == Value.Count && keys.All(k => Value.ContainsKey(k));
    public bool IsValueEqual(List<SelectOption<T>> list) => list.Count == Value.Count && list.All(o => Value.ContainsKey(o.Label));
    public bool IsValueEqual(Dictionary<string, SelectOption<T>> value) => IsValueEqual(value.Keys);

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<SelectMultiOptionEvent<T>> OnSelect { get; set; }
    public EventDelegate<SelectMultiOptionEvent<T>> OnDeselect { get; set; }
    public EventDelegate<SelectMultiCancelEvent<T>> OnCancel { get; set; }
    public EventDelegate<SelectMultiCancelEvent<T>> OnCancelClose { get; set; }
    public EventDelegate<SelectMultiEvent<T>> OnClear { get; set; }
    public EventDelegate<SelectMultiEvent<T>> OnClearClose { get; set; }
    public EventDelegate<SelectMultiEvent<T>> OnOK { get; set; }
    public EventDelegate<SelectMultiEvent<T>> OnOKClose { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SelectMultiViewModel(SelectMultiViewModelParams<T> p) : base(p.Form, p.ID, p.OnError) {
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
        OnSelect = p.OnSelect ?? EventDelegate<SelectMultiOptionEvent<T>>.EMPTY;
        OnDeselect = p.OnDeselect ?? EventDelegate<SelectMultiOptionEvent<T>>.EMPTY;
        OnCancel = p.OnCancel ?? EventDelegate<SelectMultiCancelEvent<T>>.EMPTY;
        OnCancelClose = p.OnCancelClose ?? EventDelegate<SelectMultiCancelEvent<T>>.EMPTY;
        OnClear = p.OnClear ?? EventDelegate<SelectMultiEvent<T>>.EMPTY;
        OnClearClose = p.OnClearClose ?? EventDelegate<SelectMultiEvent<T>>.EMPTY;
        OnOK = p.OnOK ?? EventDelegate<SelectMultiEvent<T>>.EMPTY;
        OnOKClose = p.OnOKClose ?? EventDelegate<SelectMultiEvent<T>>.EMPTY;
    }
}
