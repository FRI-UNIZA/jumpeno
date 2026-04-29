namespace Jumpeno.Client.Models;

public record InputSearchViewModelParams(
    string? Form = null,
    string? ID = null,
    // Value:
    InputTextMode TextMode = InputTextMode.Normal,
    InputSearchMode SearchMode = InputSearchMode.LowerCase, bool Trim = true,
    Predicate<string>? TextCheck = null,
    int? MaxLength = null,
    string? Placeholder = null, string DefaultValue = "", string ClearValue = "",
    // Events:
    EventDelegate<string>? OnSearch = null,
    Action<string>? OnError = null
);
