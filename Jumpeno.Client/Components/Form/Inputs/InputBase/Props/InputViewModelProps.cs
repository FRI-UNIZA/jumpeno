namespace Jumpeno.Client.Props;

public abstract record InputViewModelProps<T> (
    string? ID,
    string Name, string Label, string? Placeholder, bool Secret,
    INPUT_TEXT_MODE TextMode, bool Trim, Predicate<string>? TextCheck,
    int? MaxLength, int Decimals,
    T DefaultValue,
    EventDelegate<T>? OnChange
);
