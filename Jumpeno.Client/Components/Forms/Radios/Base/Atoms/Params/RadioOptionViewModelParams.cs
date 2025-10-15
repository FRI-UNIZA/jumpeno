namespace Jumpeno.Client.Models;

public record RadioOptionViewModelParams<T> (
    int Key,
    T Value,
    string Label,
    Action<string>? OnError = null
);
