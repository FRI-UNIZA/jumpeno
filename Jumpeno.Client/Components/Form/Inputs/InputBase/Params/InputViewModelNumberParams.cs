namespace Jumpeno.Client.Params;

public record InputViewModelLongParams(
    string? ID = null,
    string Name = "", string Label = "", string? Placeholder = null, bool Secret = false,
    int? MaxLength = null,
    long MinValue = long.MinValue, long MaxValue = long.MaxValue,
    long DefaultValue = 0,
    EventDelegate<long>? OnChange = null
) : InputViewModelParams<long>(
    ID, Name, Label, Placeholder, Secret, INPUT_TEXT_MODE.NORMAL, true, null, MaxLength, 0, DefaultValue, OnChange
);
