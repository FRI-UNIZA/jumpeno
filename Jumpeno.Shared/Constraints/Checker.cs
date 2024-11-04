namespace Jumpeno.Shared.Constraints;

public static class Checker {
    // Helpers ----------------------------------------------------------------------------------------------------------------------------
    private static string Name(string name) {
        if (name != "") name = $" \"{name}\"";
        return name;
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void CheckServer() {
        if (!AppEnvironment.IsServer()) throw new InvalidOperationException("Not allowed on the client!");
    }
    public static void CheckController() {
        if (!AppEnvironment.IsController()) throw new InvalidOperationException("Not allowed outside controller!");
    }
    public static void CheckClient() {
        if (AppEnvironment.IsServer()) throw new InvalidOperationException("Not allowed on the server!");
    }

    public static bool IsNotNull(object? value) {
        return value is not null;
    }
    public static void CheckNotNull(object? value, string name = "") {
        if (IsNotNull(value)) return;
        throw new InvalidDataException($"Value{Name(name)} can not be null!");
    }

    public static bool IsPrimitiveType(Type t) {
        return t.IsPrimitive || t == typeof(string);
    }
    public static void CheckPrimitiveType(Type t) {
        if (IsPrimitiveType(t)) return;
        throw new ArgumentException("Type must be primitive!");
    }

    public static bool IsEmptyString(string value, bool trim = true) {
        return (trim ? value.Trim() : value) == "";
    }
    public static void CheckEmptyString(string value, bool trim = true, string name = "") {
        if (!IsEmptyString(value, trim)) return;
        name = name == "" ? " " : $" \"{name}\" ";
        throw new ArgumentException($"Value of argument{name}is empty!");
    }

    public static bool IsLowerThan(int value1, int value2) { return value1 < value2; }
    public static void CheckLowerThan(int value1, int value2, string name = "") {
        if (IsLowerThan(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not lower than {value2}!");
    }
    public static bool IsLowerOrEqualTo(int value1, int value2) { return value1 <= value2; }
    public static void CheckLowerOrEqualTo(int value1, int value2, string name = "") {
        if (IsLowerOrEqualTo(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not lower or equal to {value2}!");
    }
    public static bool IsGreaterThan(int value1, int value2) { return value1 > value2; }
    public static void ChecGreaterThan(int value1, int value2, string name = "") {
        if (IsGreaterThan(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not greater than {value2}!");
    }
    public static bool IsGreaterOrEqualTo(int value1, int value2) { return value1 >= value2; }
    public static void CheckGreaterOrEqualTo(int value1, int value2, string name = "") {
        if (IsGreaterOrEqualTo(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not greater or equal to {value2}!");
    }

    public static bool IsAlpha(string value, List<char>? allowed = null) {
        value = value.ToUpper();
        foreach (var c in value) {
            if ((c < 'A' || 'Z' < c) && !(allowed != null && allowed.Contains(c))) {
                return false;
            }
        }
        return true;
    }
    public static bool IsAlpha(string value) { return IsAlpha(value, null); }
    public static void CheckAlpha(string value, List<char>? allowed = null, string name = "") {
        if (IsAlpha(value, allowed)) return;
        throw new ArgumentException($"Value{Name(name)} is not alphabetical!");
    }
    public static bool IsAlphaNum(string value, List<char>? allowed = null) {
        value = value.ToUpper();
        foreach (var c in value) {
            if (!char.IsDigit(c) && (c < 'A' || 'Z' < c) && !(allowed != null && allowed.Contains(c))) {
                return false;
            }
        }
        return true;
    }
    public static bool IsAlphaNum(string value) { return IsAlphaNum(value, null); }
    public static void CheckAlphaNum(string value, List<char>? allowed = null, string name = "") {
        if (IsAlphaNum(value, allowed)) return;
        throw new ArgumentException($"Value{Name(name)} is not alphanumeric!");
    }
}
