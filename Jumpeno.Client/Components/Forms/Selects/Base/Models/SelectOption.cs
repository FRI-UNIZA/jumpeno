namespace Jumpeno.Client.Models;

public record SelectOption(
    object? Value,
    string Label
) {
    public T GetValue<T>() => (T) Value!;
}
