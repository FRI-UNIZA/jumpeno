namespace Jumpeno.Client.Models;

public record SelectMultiViewModelParams (
    string? Form = null,
    string? ID = null,
    // Options:
    List<SelectOption>? Options = null,
    List<SelectOption>? DefaultValue = null,
    string? Placeholder = null,
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
    EventDelegate<SelectMultiOptionEvent>? OnSelect = null,
    EventDelegate<SelectMultiOptionEvent>? OnDeselect = null,
    EventDelegate<SelectMultiCancelEvent>? OnCancel = null,
    EventDelegate<SelectMultiCancelEvent>? OnCancelClose = null,
    EventDelegate<SelectMultiEvent>? OnClear = null,
    EventDelegate<SelectMultiEvent>? OnClearClose = null,
    EventDelegate<SelectMultiEvent>? OnOK = null,
    EventDelegate<SelectMultiEvent>? OnOKClose = null,
    Action<string>? OnError = null
);
