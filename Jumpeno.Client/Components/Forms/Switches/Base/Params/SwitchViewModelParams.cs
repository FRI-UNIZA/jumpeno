namespace Jumpeno.Client.Models;

public record SwitchViewModelParams (
    string? Form = null,
    string? ID = null,
    // Value:
    bool DefaultValue = false,
    // Events:
    EventDelegate<SwitchEvent>? OnChange = null,
    EventDelegate<SwitchEvent>? OnAfterChange = null,
    Action<string>? OnError = null
);
