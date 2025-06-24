namespace Jumpeno.Client.Models;

public record InputViewModelTextParams(
    string? Form = null,
    string? ID = null,
    INPUT_TEXT_MODE TextMode = INPUT_TEXT_MODE.NORMAL, bool Trim = false, Predicate<string>? TextCheck = null,
    int? MaxLength = null,
    string? Placeholder = null, string DefaultValue = "",
    bool Secret = false,
    EventDelegate<string>? OnChange = null, EmptyDelegate? OnEnter = null
) : InputViewModelParams<string>(
    Form, ID, TextMode, Trim, TextCheck, MaxLength, 0, Placeholder, DefaultValue,  Secret, OnChange, OnEnter
);
