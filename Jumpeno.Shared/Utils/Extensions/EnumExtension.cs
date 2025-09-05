namespace Jumpeno.Shared.Utils;

using System.Reflection;

// Attributes -----------------------------------------------------------------------------------------------------------------------------
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class StringValueAttribute(string value) : Attribute {
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class CSSClassAttribute(string value) : Attribute {
    public string Value { get; } = value;
}

// Extension ------------------------------------------------------------------------------------------------------------------------------
public static class EnumExtension {
    // StringValue:
    public static string String(this Enum value) {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<StringValueAttribute>();
        return attribute?.Value ?? value.ToString();
    }
    public static string StringLower(this Enum value) => String(value).ToLower();
    public static string StringUpper(this Enum value) => String(value).ToUpper();

    // CSSClass:
    public static string CSSClass(this Enum value) {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<CSSClassAttribute>();
        return attribute?.Value ?? value.ToString();
    }
}
