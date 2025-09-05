namespace Jumpeno.Client.Models;

public record InputViewModelDoubleParams(
    string? Form = null,
    string? ID = null,
    // Value:
    int? MaxLength = null, int Decimals = 2,
    double MinValue = double.MinValue, double MaxValue = double.MaxValue,
    string? Placeholder = null, double DefaultValue = 0.0, double ClearValue = 0.0,
    bool Secret = false,
    // Events:
    EventDelegate<InputEvent<double>>? OnInput = null,
    EventDelegate<InputEvent<double>>? OnClear = null,
    EventDelegate<InputEvent<double>>? OnChange = null,
    EventDelegate<InputEvent<double>>? OnEnter = null,
    Action<string>? OnError = null
) : InputViewModelParams<double>(
    Form, ID,
    INPUT_TEXT_MODE.NORMAL, true, null,
    MaxLength, Decimals,
    Placeholder, DefaultValue, ClearValue,
    Secret,
    OnInput, OnClear, OnChange, OnEnter, OnError
);
