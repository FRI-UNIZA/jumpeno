namespace Jumpeno.Client.Models;

public record SelectOption(
    object? Value,
    string Label
) {
    public T? GetValueEmpty<T>() => (T?) Value;
    public T GetValue<T>() => (T) Value!;
}
