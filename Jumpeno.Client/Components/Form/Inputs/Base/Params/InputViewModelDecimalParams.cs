namespace Jumpeno.Client.Params;

public record InputViewModelDoubleParams(
    string? ID = null,
    string Name = "", string Label = "", string? Placeholder = null, bool Secret = false,
    int? MaxLength = null, int Decimals = 2,
    double MinValue = double.MinValue, double MaxValue = double.MaxValue,
    double DefaultValue = 0,
    EventDelegate<double>? OnChange = null, EmptyDelegate? OnEnter = null
) : InputViewModelParams<double>(
    ID, Name, Label, Placeholder, Secret, INPUT_TEXT_MODE.NORMAL, true, null, MaxLength, Decimals, DefaultValue, OnChange, OnEnter
);
