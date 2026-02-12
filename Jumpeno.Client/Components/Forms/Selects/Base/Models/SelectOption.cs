namespace Jumpeno.Client.Models;

public record SelectOption<T>(
    int Key,
    T? Value,
    string Label
) {
    public SelectOption<T>? Pick(Func<SelectOption<T>, SelectOption<T>> pick) {
        try { return pick(this); }
        catch { return null; }
    } 
}
