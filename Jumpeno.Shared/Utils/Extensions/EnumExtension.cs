namespace Jumpeno.Shared.Utils;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class StringValueAttribute(string value) : Attribute {
    public string Value { get; } = value;
}

public static class EnumExtension {
    public static string String(this Enum value) {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (StringValueAttribute)Attribute.GetCustomAttribute(field!, typeof(StringValueAttribute))!;

        return attribute == null ? value.ToString() : attribute.Value;
    }
    public static string StringLower(this Enum value) => String(value).ToLower();
    public static string StringUpper(this Enum value) => String(value).ToUpper();
}
