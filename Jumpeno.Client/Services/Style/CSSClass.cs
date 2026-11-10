namespace Jumpeno.Client.Services;

public class CssClass {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassSurface = "surface";
    public const string ClassVariant = "variant";
    public const string ClassSize = "size";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Dictionary<string, bool> Classes = [];

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CssClass(string? className = null, bool? apply = true) {
        className = className is null || apply != true ? "" : className;
        var arr = className.Trim().Split(' ');
        for (int i = 0; i < arr.Length; i++) {
            var value = arr[i].Trim();
            if (value == "") continue;
            Classes[value] = true;
        }
    }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool Contains(string className) {
        Classes.TryGetValue(className.Trim(), out var value);
        return value;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    // Set class:
    public CssClass SetSurface<T>(T? prop, bool? apply = true) { Set(ClassSurface, prop != null && apply == true); return Set(((Surface?)(dynamic?)prop)?.CSSClass(), apply); }
    public CssClass SetVariant(Enum? prop, bool? apply = true) { Set(ClassVariant, prop != null && apply == true); return Set(prop?.CSSClass(), apply); }
    public CssClass SetSize(Enum? prop, bool? apply = true) { Set(ClassSize, prop != null && apply == true); return Set(prop?.CSSClass(), apply); }
    public CssClass Set(Enum? prop, bool? apply = true) => Set(prop?.CSSClass(), apply);
    public CssClass Set(string? className, bool? apply = true) {
        if (className == null || apply != true) return this;
        var c = new CssClass(className);
        foreach (var @class in c.Classes) {
            Classes[@class.Key] = true;
        }
        return this;
    }

    // Remove class:
    public CssClass RemoveSurface<T>(T? prop, bool? apply = true) { Remove(((Surface?)(dynamic?)prop)?.CSSClass(), apply); return Remove(ClassSurface, apply); }
    public CssClass RemoveVariant(Enum? prop, bool? apply = true) { Remove(prop?.CSSClass(), apply); return Remove(ClassVariant, apply); }
    public CssClass RemoveSize(Enum? prop, bool? apply = true) { Remove(prop?.CSSClass(), apply); return Remove(ClassSize, apply); }
    public CssClass Remove(Enum? prop, bool? apply = true) => Remove(prop?.CSSClass(), apply);
    public CssClass Remove(string? className, bool? apply = true) { if (className != null && apply == true) Classes.Remove(className.Trim()); return this; }

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public override string ToString() {
        var value = "";
        foreach (var @class in Classes) {
            value = $"{value} {@class.Key}";
        }
        return value.Trim();
    }

    public static implicit operator string(CssClass instance) {
        return instance.Classes.Count < 1 ? null! : instance.ToString();
    }
}
