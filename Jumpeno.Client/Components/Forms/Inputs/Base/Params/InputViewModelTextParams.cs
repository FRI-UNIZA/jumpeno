namespace Jumpeno.Client.Models;

public record InputViewModelTextParams(
    string? Form = null,
    string? ID = null,
    // Value:
    INPUT_TEXT_MODE TextMode = INPUT_TEXT_MODE.NORMAL, bool Trim = false, Predicate<string>? TextCheck = null,
    int? MaxLength = null,
    string? Placeholder = null, string DefaultValue = "", string ClearValue = "",
    bool Secret = false,
    // Events:
    EventDelegate<InputEvent<string>>? OnInput = null,
    EventDelegate<InputEvent<string>>? OnClear = null,
    EventDelegate<InputEvent<string>>? OnChange = null,
    EventDelegate<InputEvent<string>>? OnEnter = null,
    Action<string>? OnError = null
) : InputViewModelParams<string>(
    Form, ID,
    TextMode, Trim, TextCheck,
    MaxLength, 0,
    Placeholder, DefaultValue, ClearValue,
    Secret,
    OnInput, OnClear, OnChange, OnEnter, OnError
);
