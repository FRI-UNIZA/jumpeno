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
    InputTextMode SearchTextMode = InputTextMode.Normal,
    InputSearchMode SearchMode = InputSearchMode.LowerCase,
    bool SearchTrim = true,
    Predicate<string>? SearchTextCheck = null,
    int? SearchMaxLength = null,
    // Events:
    EventDelegate<SelectEvent<T>>? OnSelect = null,
    EventDelegate<SelectEvent<T>>? OnCloseSelected = null,
    EventDelegate<SelectEvent<T>>? OnAfterCloseSelected = null,
    Action<string>? OnError = null
);
