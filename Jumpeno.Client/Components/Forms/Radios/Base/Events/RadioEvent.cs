namespace Jumpeno.Client.Models;

public record RadioEvent<T> (
    RadioOptionViewModel<T>? Before,
    RadioOptionViewModel<T>? After
) {}
