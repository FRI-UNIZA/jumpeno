namespace Jumpeno.Client.Models;

public record SelectMultiViewModelParams<T> (
    string? Form = null,
    string? ID = null,
    // Options:
    List<SelectOption<T>>? Options = null,
    List<SelectOption<T>>? DefaultValue = null,
    string? Placeholder = null,
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
    EventDelegate<SelectMultiOptionEvent<T>>? OnSelect = null,
    EventDelegate<SelectMultiOptionEvent<T>>? OnDeselect = null,
    EventDelegate<SelectMultiCancelEvent<T>>? OnCancel = null,
    EventDelegate<SelectMultiCancelEvent<T>>? OnCancelClose = null,
    EventDelegate<SelectMultiEvent<T>>? OnClear = null,
    EventDelegate<SelectMultiEvent<T>>? OnClearClose = null,
    EventDelegate<SelectMultiEvent<T>>? OnOK = null,
    EventDelegate<SelectMultiEvent<T>>? OnOKClose = null,
    Action<string>? OnError = null
);
