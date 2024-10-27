namespace Jumpeno.Client.Models;

public record SelectOption(
    object? Value,
    string Label
) {
    public T? GetValueEmpty<T>() { return (T?) Value; }
    public T GetValue<T>() { return (T) Value!; }
}
