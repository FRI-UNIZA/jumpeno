namespace Jumpeno.Client.Models;

public record SelectViewModelParams (
    string? Form = null,
    string? ID = null,
    List<SelectOption>? Options = null,
    SelectOption? DefaultValue = null,
    bool Empty = false,
    EventDelegate<SelectEvent>? OnSelect = null,
    EventDelegate<SelectEvent>? OnCloseSelected = null
);
