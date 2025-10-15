namespace Jumpeno.Client.Models;

public record SelectSearchEvent<T> (
    string Search,
    SelectOption<T> Option
) {}
