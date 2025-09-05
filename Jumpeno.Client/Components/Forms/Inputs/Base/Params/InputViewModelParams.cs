namespace Jumpeno.Client.Models;

public abstract record InputViewModelParams<T> (
    string? Form,
    string? ID,
    // Value:
    INPUT_TEXT_MODE TextMode, bool Trim, Predicate<string>? TextCheck,
    int? MaxLength, int Decimals,
    string? Placeholder, T DefaultValue, T ClearValue,
    bool Secret,
    // Events:
    EventDelegate<InputEvent<T>>? OnInput,
    EventDelegate<InputEvent<T>>? OnClear,
    EventDelegate<InputEvent<T>>? OnChange,
    EventDelegate<InputEvent<T>>? OnEnter,
    Action<string>? OnError
);
