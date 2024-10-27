namespace Jumpeno.Shared.Extensions;

public static class EnumExtensions {
    public static string StringValue(this Enum value) {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (StringValueAttribute)Attribute.GetCustomAttribute(field!, typeof(StringValueAttribute))!;

        return attribute == null ? value.ToString() : attribute.Value;
    }
}
