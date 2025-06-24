namespace Jumpeno.Client.Models;

public abstract record InputViewModelParams<T> (
    string? Form,
    string? ID,
    INPUT_TEXT_MODE TextMode, bool Trim, Predicate<string>? TextCheck,
    int? MaxLength, int Decimals,
    string? Placeholder, T DefaultValue,
    bool Secret,
    EventDelegate<T>? OnChange, EmptyDelegate? OnEnter
);
