namespace Jumpeno.Client.Models;

public record TextAreaViewModelParams(
    string? Form = null,
    string? ID = null,
    // Value:
    INPUT_TEXT_MODE TextMode = INPUT_TEXT_MODE.NORMAL,
    bool Trim = false,
    Predicate<string>? TextCheck = null,
    int? MaxLength = null,
    int? Rows = null,
    bool AutoResize = false,
    string? Placeholder = null,
    string DefaultValue = "",
    string ClearValue = "",
    // Events:
    EventDelegate<InputEvent<string>>? OnInput = null,
    EventDelegate<InputEvent<string>>? OnClear = null,
    EventDelegate<InputEvent<string>>? OnChange = null,
    EventDelegate<InputEvent<string>>? OnEnter = null,
    Action<string>? OnError = null
);
