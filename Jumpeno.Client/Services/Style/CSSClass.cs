namespace Jumpeno.Client.Services;

public class CSSClass {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS_SURFACE = "surface";
    public const string CLASS_VARIANT = "variant";
    public const string CLASS_SIZE = "size";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Dictionary<string, bool> Classes = [];

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CSSClass(string? className = null, bool? apply = true) {
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
    public CSSClass SetSurface<T>(T? prop, bool? apply = true) { Set(CLASS_SURFACE, prop != null && apply == true); return Set(((SURFACE?)(dynamic?)prop)?.CSSClass(), apply); }
    public CSSClass SetVariant(Enum? prop, bool? apply = true) { Set(CLASS_VARIANT, prop != null && apply == true); return Set(prop?.CSSClass(), apply); }
    public CSSClass SetSize(Enum? prop, bool? apply = true) { Set(CLASS_SIZE, prop != null && apply == true); return Set(prop?.CSSClass(), apply); }
    public CSSClass Set(Enum? prop, bool? apply = true) => Set(prop?.CSSClass(), apply);
    public CSSClass Set(string? className, bool? apply = true) {
        if (className == null || apply != true) return this;
        var c = new CSSClass(className);
        foreach (var @class in c.Classes) {
            Classes[@class.Key] = true;
        }
        return this;
    }

    // Remove class:
    public CSSClass RemoveSurface<T>(T? prop, bool? apply = true) { Remove(((SURFACE?)(dynamic?)prop)?.CSSClass(), apply); return Remove(CLASS_SURFACE, apply); }
    public CSSClass RemoveVariant(Enum? prop, bool? apply = true) { Remove(prop?.CSSClass(), apply); return Remove(CLASS_VARIANT, apply); }
    public CSSClass RemoveSize(Enum? prop, bool? apply = true) { Remove(prop?.CSSClass(), apply); return Remove(CLASS_SIZE, apply); }
    public CSSClass Remove(Enum? prop, bool? apply = true) => Remove(prop?.CSSClass(), apply);
    public CSSClass Remove(string? className, bool? apply = true) { if (className != null && apply == true) Classes.Remove(className.Trim()); return this; }

    // Operators --------------------------------------------------------------------------------------------------------------------------
    public override string ToString() {
        var value = "";
        foreach (var @class in Classes) {
            value = $"{value} {@class.Key}";
        }
        return value.Trim();
    }

    public static implicit operator string(CSSClass instance) {
        return instance.Classes.Count < 1 ? null! : instance.ToString();
    }
}
