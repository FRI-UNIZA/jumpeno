namespace Jumpeno.Client.Params;

public record InputViewModelTextParams(
    string? ID = null,
    string Name = "", string Label = "", string? Placeholder = null, bool Secret = false,
    INPUT_TEXT_MODE TextMode = INPUT_TEXT_MODE.NORMAL, bool Trim = false, Predicate<string>? TextCheck = null,
    int? MaxLength = null,
    string DefaultValue = "",
    EventDelegate<string>? OnChange = null
) : InputViewModelParams<string>(
    ID, Name, Label, Placeholder, Secret, TextMode, Trim, TextCheck, MaxLength, 0, DefaultValue, OnChange
);
