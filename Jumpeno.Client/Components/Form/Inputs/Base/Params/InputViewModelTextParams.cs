namespace Jumpeno.Client.Models;

public record InputViewModelTextParams(
    string? Form = null,
    string? ID = null,
    string Name = "", string Label = "", string? Placeholder = null, bool Secret = false,
    INPUT_TEXT_MODE TextMode = INPUT_TEXT_MODE.NORMAL, bool Trim = false, Predicate<string>? TextCheck = null,
    int? MaxLength = null,
    string DefaultValue = "",
    EventDelegate<string>? OnChange = null, EmptyDelegate? OnEnter = null
) : InputViewModelParams<string>(
    Form, ID, Name, Label, Placeholder, Secret, TextMode, Trim, TextCheck, MaxLength, 0, DefaultValue, OnChange, OnEnter
);
