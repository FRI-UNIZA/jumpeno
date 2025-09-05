namespace Jumpeno.Client.Models;

public record InputSearchViewModelParams(
    string? Form = null,
    string? ID = null,
    // Value:
    INPUT_TEXT_MODE TextMode = INPUT_TEXT_MODE.NORMAL,
    INPUT_SEARCH_MODE SearchMode = INPUT_SEARCH_MODE.LOWERCASE, bool Trim = true,
    Predicate<string>? TextCheck = null,
    int? MaxLength = null,
    string? Placeholder = null, string DefaultValue = "", string ClearValue = "",
    // Events:
    EventDelegate<string>? OnSearch = null,
    Action<string>? OnError = null
);
