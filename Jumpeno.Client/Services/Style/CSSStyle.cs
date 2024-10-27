namespace Jumpeno.Client.Services;

public class CSSStyle {
    // Conversions ------------------------------------------------------------------------------------------------------------------------
    public static string Unit(string value) {
        value = value.Trim();
        var unit = "";
        for (var i = value.Length - 1; i >= 0; i--) {
            if (char.IsDigit(value[i])) break;
            else unit = $"{value[i]}{unit}";
        }
        return unit.Trim();
    }
    public static string Value(string value) { return value.Replace(Unit(value)!, "").Trim(); }
    public static int IntValue(string value) { return int.Parse(Value(value)) ; }
    public static double DoubleValue(string value) { return double.Parse(Value(value)); }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Dictionary<string, string> Styles = [];

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public CSSStyle(string? styles) {
        styles = styles is null ? "" : styles;
        var arr = styles.Split(';');
        for (int i = 0; i < arr.Length; i++) {
            var index = arr[i].LastIndexOf(':');
            if (index < 0) continue;
            var property = arr[i].Substring(0, index).Trim();
            var value = arr[i].Substring(index + 1).Trim();
            if (property == "" || value == "") continue;
            Styles[property] = value;
        }
    }
    public CSSStyle(): this("") {}

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public string? Get(string property) {
        Styles.TryGetValue(property.Trim(), out var value);
        return value;
    }
    public string? GetUnit(string property) {
        string? value = Get(property);
        if (value is null) return null;
        return Unit(value);
    }
    public int? GetInt(string property) {
        string? value = Get(property);
        if (value is null) return null;
        return IntValue(value);
    }
    public double? GetDouble(string property) {
        string? value = Get(property);
        if (value is null) return null;
        return DoubleValue(value);
    }

    public void Set(string property, string value) {
        property = property.Trim();
        value = value.Trim();
        if (property == "" || value == "") return;
        Styles[property] = value;
    }

    public bool Remove(string property) {
        return Styles.Remove(property.Trim());
    }

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public override string ToString() {
        var value = "";
        foreach (var block in Styles) {
            value = $"{value} {block.Key}: {block.Value};";
        }
        return value.Trim();
    }

    public static implicit operator string(CSSStyle instance) {
        return instance.Styles.Count < 1 ? null! : instance.ToString();
    }
}
