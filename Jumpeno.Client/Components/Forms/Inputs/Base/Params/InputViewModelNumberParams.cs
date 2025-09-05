namespace Jumpeno.Client.Models;

public record InputViewModelLongParams(
    string? Form = null,
    string? ID = null,
    // Value:
    int? MaxLength = null,
    long MinValue = long.MinValue, long MaxValue = long.MaxValue,
    string? Placeholder = null, long DefaultValue = 0, long ClearValue = 0,
    bool Secret = false,
    // Events:
    EventDelegate<InputEvent<long>>? OnInput = null,
    EventDelegate<InputEvent<long>>? OnClear = null,
    EventDelegate<InputEvent<long>>? OnChange = null,
    EventDelegate<InputEvent<long>>? OnEnter = null,
    Action<string>? OnError = null
) : InputViewModelParams<long>(
    Form, ID,
    INPUT_TEXT_MODE.NORMAL, true, null,
    MaxLength, 0,
    Placeholder, DefaultValue, ClearValue,
    Secret,
    OnInput, OnClear, OnChange, OnEnter, OnError
);
