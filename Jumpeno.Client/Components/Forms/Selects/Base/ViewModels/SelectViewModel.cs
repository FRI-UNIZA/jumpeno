namespace Jumpeno.Client.ViewModels;

public class SelectViewModel<T> : FormViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public readonly Predicate<SelectSearchEvent<T>> DEFAULT_CUSTOM_SEARCH = e => e.Option.Label.ToLower().IndexOf(e.Search) >= 0;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Options:
    public readonly List<SelectOption<T>> Options;
    public readonly SelectOption<T> DefaultValue;
    public readonly string? Placeholder;
    public readonly bool Empty;
    // Search:
    public readonly bool Search;
    public readonly Predicate<SelectSearchEvent<T>> CustomSearch;
    // Search input:
    public readonly InputSearchViewModel SearchVM;
    // Value:
    public SelectOption<T> Value { get; private set; }
    public void SetValue(SelectOption<T> option) { if (option != Value) { Value = option; Error.Clear(); } }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<SelectEvent<T>> OnSelect { get; set; }
    public EventDelegate<SelectEvent<T>> OnCloseSelected { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SelectViewModel(SelectViewModelParams<T> p) : base(p.Form, p.ID, p.OnError) {
        if (p.Options == null || (!p.Empty && p.Options.Count < 1)) {
            throw new InvalidDataException("Empty select options!");
        }
        // Options:
        Options = p.Options;
        DefaultValue = p.DefaultValue ?? (p.Empty ? SELECT<T>.EMPTY_OPTION : Options[0]);
        Placeholder = p.Placeholder;
        Empty = p.Empty;
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
        Value = DefaultValue;
        // Events:
        OnSelect = p.OnSelect ?? EventDelegate<SelectEvent<T>>.EMPTY;
        OnCloseSelected = p.OnCloseSelected ?? EventDelegate<SelectEvent<T>>.EMPTY;
    }
}
