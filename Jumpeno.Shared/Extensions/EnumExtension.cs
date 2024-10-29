namespace Jumpeno.Shared.Extensions;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class StringValueAttribute(string value) : Attribute {
    public string Value { get; } = value;
}

public static class EnumExtensions {
    public static string StringValue(this Enum value) {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (StringValueAttribute)Attribute.GetCustomAttribute(field!, typeof(StringValueAttribute))!;

        return attribute == null ? value.ToString() : attribute.Value;
    }
}
