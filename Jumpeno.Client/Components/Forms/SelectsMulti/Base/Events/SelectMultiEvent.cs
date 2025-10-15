namespace Jumpeno.Client.Models;

public record SelectMultiEvent<T> (
    Dictionary<string, SelectOption<T>> Before,
    Dictionary<string, SelectOption<T>> After
) {}
