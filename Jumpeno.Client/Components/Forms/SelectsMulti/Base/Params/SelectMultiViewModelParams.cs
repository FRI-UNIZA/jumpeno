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
    InputTextMode SearchTextMode = InputTextMode.NORMAL,
    InputSearchMode SearchMode = InputSearchMode.LOWERCASE,
    bool SearchTrim = true,
    Predicate<string>? SearchTextCheck = null,
    int? SearchMaxLength = null,
    // Events:
    EventDelegate<SelectMultiOptionEvent<T>>? OnSelect = null,
    EventDelegate<SelectMultiOptionEvent<T>>? OnDeselect = null,
    EventDelegate<SelectMultiCancelEvent<T>>? OnCancel = null,
    EventDelegate<SelectMultiCancelEvent<T>>? OnCancelClose = null,
    EventDelegate<SelectMultiCancelEvent<T>>? OnAfterCancelClose = null,
    EventDelegate<SelectMultiEvent<T>>? OnClear = null,
    EventDelegate<SelectMultiEvent<T>>? OnClearClose = null,
    EventDelegate<SelectMultiEvent<T>>? OnAfterClearClose = null,
    EventDelegate<SelectMultiEvent<T>>? OnOK = null,
    EventDelegate<SelectMultiEvent<T>>? OnOKClose = null,
    EventDelegate<SelectMultiEvent<T>>? OnAfterOKClose = null,
    Action<string>? OnError = null
);
