namespace Jumpeno.Shared.Utils;

public static class Checker {
    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private static string Name(string name) {
        if (name != "") name = $" \"{name}\"";
        return name;
    }

    // General ----------------------------------------------------------------------------------------------------------------------------
    public static void Check(bool condition, Error error) => Check(Validate(condition, error));
    public static void Check(List<Error> errors) { if (errors.Count > 0) throw new CoreException(errors); }

    public static List<Error> Validate(bool condition, Error error) {
        List<Error> errors = [];
        Validate(errors, condition, error);
        return errors;
    }
    public static void Validate(List<Error> errors, bool condition, Error error) { if (condition) errors.Add(error); }

    // Null -------------------------------------------------------------------------------------------------------------------------------
    public static bool IsNotNull(object? value) {
        return value is not null;
    }
    public static void CheckNotNull(object? value, string name = "") {
        if (IsNotNull(value)) return;
        throw new InvalidDataException($"Value{Name(name)} can not be null!");
    }

    // Disposable -------------------------------------------------------------------------------------------------------------------------
    public static bool IsDisposableSync(object? value) => value is IDisposable;
    public static bool IsDisposableAsync(object? value) => value is IAsyncDisposable;
    public static bool IsDisposable(object? value) => IsDisposableSync(value) || IsDisposableAsync(value);

    public static void CheckDisposable(object? value) {
        if (IsDisposable(value)) return;
        throw new InvalidDataException("Object not disposable!");
    }

    // Primitives -------------------------------------------------------------------------------------------------------------------------
    public static bool IsPrimitiveType(Type t) {
        return t.IsPrimitive || t == typeof(string);
    }
    public static void CheckPrimitiveType(Type t) {
        if (IsPrimitiveType(t)) return;
        throw new ArgumentException("Type must be primitive!");
    }

    // Empty ------------------------------------------------------------------------------------------------------------------------------
    public static bool IsEmptyString(string value, bool trim = true) {
        return (trim ? value.Trim() : value) == "";
    }
    public static void CheckEmptyString(string value, bool trim = true, string name = "") {
        if (!IsEmptyString(value, trim)) return;
        name = name == "" ? " " : $" \"{name}\" ";
        throw new ArgumentException($"Value of argument{name}is empty!");
    }

    // Whole numbers ----------------------------------------------------------------------------------------------------------------------
    public static bool IsLowerThan(int value1, int value2) => value1 < value2;
    public static void CheckLowerThan(int value1, int value2, string name = "") {
        if (IsLowerThan(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not lower than {value2}!");
    }
    public static bool IsLowerOrEqualTo(int value1, int value2) => value1 <= value2;
    public static void CheckLowerOrEqualTo(int value1, int value2, string name = "") {
        if (IsLowerOrEqualTo(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not lower or equal to {value2}!");
    }
    public static bool IsGreaterThan(int value1, int value2) => value1 > value2;
    public static void CheckGreaterThan(int value1, int value2, string name = "") {
        if (IsGreaterThan(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not greater than {value2}!");
    }
    public static bool IsGreaterOrEqualTo(int value1, int value2) => value1 >= value2;
    public static void CheckGreaterOrEqualTo(int value1, int value2, string name = "") {
        if (IsGreaterOrEqualTo(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not greater or equal to {value2}!");
    }

    // Decimal numbers --------------------------------------------------------------------------------------------------------------------
    public static bool IsLowerThan(double value1, double value2) => value1 < value2;
    public static void CheckLowerThan(double value1, double value2, string name = "") {
        if (IsLowerThan(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not lower than {value2}!");
    }
    public static bool IsLowerOrEqualTo(double value1, double value2) => value1 <= value2;
    public static void CheckLowerOrEqualTo(double value1, double value2, string name = "") {
        if (IsLowerOrEqualTo(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not lower or equal to {value2}!");
    }
    public static bool IsGreaterThan(double value1, double value2) => value1 > value2;
    public static void CheckGreaterThan(double value1, double value2, string name = "") {
        if (IsGreaterThan(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not greater than {value2}!");
    }
    public static bool IsGreaterOrEqualTo(double value1, double value2) => value1 >= value2;
    public static void CheckGreaterOrEqualTo(double value1, double value2, string name = "") {
        if (IsGreaterOrEqualTo(value1, value2)) return;
        throw new ArgumentOutOfRangeException($"Value{Name(name)} is not greater or equal to {value2}!");
    }

    // String values ----------------------------------------------------------------------------------------------------------------------
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
