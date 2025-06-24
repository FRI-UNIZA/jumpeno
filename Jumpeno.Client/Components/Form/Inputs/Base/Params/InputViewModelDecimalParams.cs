namespace Jumpeno.Client.Models;

public record InputViewModelDoubleParams(
    string? Form = null,
    string? ID = null,
    int? MaxLength = null, int Decimals = 2,
    double MinValue = double.MinValue, double MaxValue = double.MaxValue,
    string? Placeholder = null, double DefaultValue = 0,
    bool Secret = false,
    EventDelegate<double>? OnChange = null, EmptyDelegate? OnEnter = null
) : InputViewModelParams<double>(
    Form, ID, INPUT_TEXT_MODE.NORMAL, true, null, MaxLength, Decimals, Placeholder, DefaultValue, Secret, OnChange, OnEnter
);
