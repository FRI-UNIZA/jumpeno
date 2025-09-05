namespace Jumpeno.Client.Models;

public record CheckBoxViewModelParams (
    string? Form = null,
    string? ID = null,
    // Value:
    bool DefaultValue = false,
    // Events:
    EventDelegate<CheckBoxEvent>? OnChange = null,
    EventDelegate<CheckBoxEvent>? OnAfterChange = null,
    Action<string>? OnError = null
);
