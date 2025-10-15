namespace Jumpeno.Client.Models;

public record SelectViewModelParams<T> (
    string? Form = null,
    string? ID = null,
    // Options:
    List<SelectOption<T>>? Options = null,
    SelectOption<T>? DefaultValue = null,
    string? Placeholder = null,
    bool Empty = false,
    // Search:
    bool Search = false,
    Predicate<SelectSearchEvent<T>>? CustomSearch = null,
    // Search input:
    INPUT_TEXT_MODE SearchTextMode = INPUT_TEXT_MODE.NORMAL,
    INPUT_SEARCH_MODE SearchMode = INPUT_SEARCH_MODE.LOWERCASE,
    bool SearchTrim = true,
    Predicate<string>? SearchTextCheck = null,
    int? SearchMaxLength = null,
    // Events:
    EventDelegate<SelectEvent<T>>? OnSelect = null,
    EventDelegate<SelectEvent<T>>? OnCloseSelected = null,
    Action<string>? OnError = null
);
