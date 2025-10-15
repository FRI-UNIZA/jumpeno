namespace Jumpeno.Client.Models;

public record SelectEvent<T> (
    SelectOption<T> Before,
    SelectOption<T> After
) {}
