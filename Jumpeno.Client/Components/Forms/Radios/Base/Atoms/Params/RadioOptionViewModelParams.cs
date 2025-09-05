namespace Jumpeno.Client.Models;

public record RadioOptionViewModelParams<T> (
    T Value,
    string Label,
    Action<string>? OnError = null
);
