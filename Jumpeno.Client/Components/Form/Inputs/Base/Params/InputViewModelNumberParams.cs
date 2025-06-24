namespace Jumpeno.Client.Models;

public record InputViewModelLongParams(
    string? Form = null,
    string? ID = null,
    int? MaxLength = null,
    long MinValue = long.MinValue, long MaxValue = long.MaxValue,
    string? Placeholder = null, long DefaultValue = 0,
    bool Secret = false,
    EventDelegate<long>? OnChange = null, EmptyDelegate? OnEnter = null
) : InputViewModelParams<long>(
    Form, ID, INPUT_TEXT_MODE.NORMAL, true, null, MaxLength, 0, Placeholder, DefaultValue, Secret, OnChange, OnEnter
);
