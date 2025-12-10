namespace Jumpeno.Client.Models;

public class RadioOptionDTO<T>(int key, T value, string label) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public int Key { get; set; } = key;
    public T Value { get; set; } = value;
    public string Label { get; set; } = label;

    // Comparator -------------------------------------------------------------------------------------------------------------------------
    public override bool Equals(object? obj) {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not RadioOptionDTO<T> other) return false;
        return Key == other.Key
            && EqualityComparer<T>.Default.Equals(Value, other.Value)
            && string.Equals(Label, other.Label, StringComparison.Ordinal);
    }

    public override int GetHashCode() => HashCode.Combine(Key, Value, Label);

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public static bool operator ==(RadioOptionDTO<T>? left, RadioOptionDTO<T>? right) => Equals(left, right);

    public static bool operator !=(RadioOptionDTO<T>? left, RadioOptionDTO<T>? right) => !Equals(left, right);

    // Pick -------------------------------------------------------------------------------------------------------------------------------
    public RadioOptionViewModel<T>? Pick(Func<RadioOptionDTO<T>, RadioOptionViewModel<T>> pick) {
        try { return pick(this); }
        catch { return null; }
    }
}
