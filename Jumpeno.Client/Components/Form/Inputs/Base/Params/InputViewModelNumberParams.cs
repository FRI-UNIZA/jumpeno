namespace Jumpeno.Client.Models;

public record InputViewModelLongParams(
    string? Form = null,
    string? ID = null,
    string Name = "", string Label = "", string? Placeholder = null, bool Secret = false,
    int? MaxLength = null,
    long MinValue = long.MinValue, long MaxValue = long.MaxValue,
    long DefaultValue = 0,
    EventDelegate<long>? OnChange = null, EmptyDelegate? OnEnter = null
) : InputViewModelParams<long>(
    Form, ID, Name, Label, Placeholder, Secret, INPUT_TEXT_MODE.NORMAL, true, null, MaxLength, 0, DefaultValue, OnChange, OnEnter
);
