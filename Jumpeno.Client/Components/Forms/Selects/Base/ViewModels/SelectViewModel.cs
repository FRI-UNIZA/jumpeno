namespace Jumpeno.Client.ViewModels;

public class SelectViewModel : FormViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public readonly Predicate<SelectSearchEvent> DEFAULT_CUSTOM_SEARCH = e => e.Option.Label.ToLower().IndexOf(e.Search) >= 0;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    // Options:
    public readonly List<SelectOption> Options;
    public readonly SelectOption DefaultValue;
    public readonly string? Placeholder;
    public readonly bool Empty;
    // Search:
    public readonly bool Search;
    public readonly Predicate<SelectSearchEvent> CustomSearch;
    // Search input:
    public readonly InputSearchViewModel SearchVM;
    // Value:
    public SelectOption Value { get; private set; }
    public void SetValue(SelectOption option) { if (option != Value) { Value = option; Error.Clear(); } }

    // Events -----------------------------------------------------------------------------------------------------------------------------
    public EventDelegate<SelectEvent> OnSelect { get; set; }
    public EventDelegate<SelectEvent> OnCloseSelected { get; set; }

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public SelectViewModel(SelectViewModelParams p) : base(p.Form, p.ID, p.OnError) {
        if (p.Options == null || (!p.Empty && p.Options.Count < 1)) {
            throw new InvalidDataException("Empty select options!");
        }
        // Options:
        Options = p.Options;
        DefaultValue = p.DefaultValue ?? (p.Empty ? SELECT.EMPTY_OPTION : Options[0]);
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
        OnSelect = p.OnSelect ?? EventDelegate<SelectEvent>.EMPTY;
        OnCloseSelected = p.OnCloseSelected ?? EventDelegate<SelectEvent>.EMPTY;
    }
}
