namespace Jumpeno.Client.Services;

public class CSSClass {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Dictionary<string, bool> Classes = [];

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public CSSClass(string? className) {
        className = className is null ? "" : className;
        var arr = className.Trim().Split(' ');
        for (int i = 0; i < arr.Length; i++) {
            var value = arr[i].Trim();
            if (value == "") continue;
            Classes[value] = true;
        }
    }
    public CSSClass(): this("") {}
    
    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public bool Contains(string className) {
        Classes.TryGetValue(className.Trim(), out var value);
        return value;
    }

    public void Set(string className) {
        var c = new CSSClass(className);
        foreach (var @class in c.Classes) {
            Classes[@class.Key] = true;
        }
    }

    public bool Remove(string className) {
        return Classes.Remove(className.Trim());
    }

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public override string ToString() {
        var value = "";
        foreach (var className in Classes) {
            value = $"{value} {className.Key}";
        }
        return value.Trim();
    }

    public static implicit operator string(CSSClass instance) {
        return instance.Classes.Count < 1 ? null! : instance.ToString();
    }
}
