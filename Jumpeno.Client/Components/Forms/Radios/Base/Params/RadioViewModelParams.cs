namespace Jumpeno.Client.Models;

public record RadioViewModelParams<T> (
    string? Form = null,
    string? ID = null,
    // Value:
    RadioOptionViewModel<T>? DefaultValue = null,
    // Events:
    EventDelegate<RadioEvent<T>>? OnChange = null,
    EventDelegate<RadioEvent<T>>? OnAfterChange = null,
    Action<string>? OnError = null
);
