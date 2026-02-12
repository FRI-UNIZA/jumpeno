namespace Jumpeno.Client.Models;

public record SelectMultiCancelEvent<T> (
    Dictionary<string, SelectOption<T>> Cancelled,
    Dictionary<string, SelectOption<T>> Value
) {}
