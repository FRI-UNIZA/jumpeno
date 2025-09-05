namespace Jumpeno.Client.Models;

public record SelectViewModelParams (
    string? Form = null,
    string? ID = null,
    // Options:
    List<SelectOption>? Options = null,
    SelectOption? DefaultValue = null,
    string? Placeholder = null,
    bool Empty = false,
    // Search:
    bool Search = false,
    Predicate<SelectSearchEvent>? CustomSearch = null,
    // Search input:
    INPUT_TEXT_MODE SearchTextMode = INPUT_TEXT_MODE.NORMAL,
    INPUT_SEARCH_MODE SearchMode = INPUT_SEARCH_MODE.LOWERCASE,
    bool SearchTrim = true,
    Predicate<string>? SearchTextCheck = null,
    int? SearchMaxLength = null,
    // Events:
    EventDelegate<SelectEvent>? OnSelect = null,
    EventDelegate<SelectEvent>? OnCloseSelected = null,
    Action<string>? OnError = null
);
