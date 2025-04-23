namespace Jumpeno.Shared.Utils;

using System.Text.RegularExpressions;

public static class Checker {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Message:
    public const string MESSAGE_ERROR = "Something went wrong.";
    public const string MESSAGE_VALUES = "Incorrect field values.";
    // Field:
    public const string FIELD_UNDEFINED = "Value undefined";
    public const string FIELD_EMPTY = "Empty field";
    public const string FIELD_FORMAT = "Wrong format";
    public const string FIELD_NOT_MATCH = "Not a match";
    public const string FIELD_EXISTS = "Already exists";

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    private static string Name(string name) {
        if (name != "") name = $" \"{name}\"";
        return name;
    }

    // General ----------------------------------------------------------------------------------------------------------------------------
    public static void Check(bool condition, Error error, string? message = null) {
        Check(Validate(condition, error), message);
    }
    public static void Check(List<Error> errors, string? message = null) {
        if (errors.Count > 0) throw new CoreException(errors).SetMessage(message ?? MESSAGE_ERROR);
    }
    public static void CheckValues(bool condition, Error error, string? message = null) {
        try { Check(condition, error, message ?? MESSAGE_VALUES); }
        catch (CoreException e) { e.SetCode(400); throw; }
    }
    public static void CheckValues(List<Error> errors, string? message = null) {
        try { Check(errors, message ?? MESSAGE_VALUES); }
        catch (CoreException e) { e.SetCode(400); throw; }
    }

    public static List<Error> Validate(bool condition, Error error) {
        List<Error> errors = [];
        Validate(errors, condition, error);
        return errors;
    }
    public static void Validate(List<Error> errors, bool condition, Error error) { if (condition) errors.Add(error); }

    // Null -------------------------------------------------------------------------------------------------------------------------------
    public static bool IsNotNull(object? value) => value is not null;
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
    public static List<Error> ValidateUndefined(string id, object? value) {
        return Validate(
            value == null,
            new Error(id, FIELD_UNDEFINED)
        );
    }
    public static List<Error> ValidateEmpty(string id, object? value) {
        return Validate(
            value == null || (value is string s && s.Trim() == ""),
            new Error(id, FIELD_EMPTY)
        );
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
    public static bool IsAlpha(string value) => IsAlpha(value, null);
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
    public static bool IsAlphaNum(string value) => IsAlphaNum(value, null);
    public static void CheckAlphaNum(string value, List<char>? allowed = null, string name = "") {
        if (IsAlphaNum(value, allowed)) return;
        throw new ArgumentException($"Value{Name(name)} is not alphanumeric!");
    }

    // Email ------------------------------------------------------------------------------------------------------------------------------
    public static bool IsValidEmail(string value) {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value.Length > EMAIL.MAX_LENGTH) return false;
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(value, pattern);
    }
    public static void CheckValidEmail(string value, string name = "") {
        if (IsValidEmail(value)) return;
        throw new ArgumentException($"Value{Name(name)} is not valid email!");
    }
    public static bool IsEmail(string value) => IsAlphaNum(value, ['@', '.']);
    public static void CheckEmail(string value, string name = "") {
        if (IsEmail(value)) return;
        throw new ArgumentException($"Value{Name(name)} is not email string!");
    }

    // Password ---------------------------------------------------------------------------------------------------------------------------
    public static bool IsPassword(string value) => IsAlphaNum(value, ['.', ',', '-', '_', '@']);
    public static void CheckPassword(string value, string name = "") {
        if (IsPassword(value)) return;
        throw new ArgumentException($"Value{Name(name)} is not a password!");
    }
}
