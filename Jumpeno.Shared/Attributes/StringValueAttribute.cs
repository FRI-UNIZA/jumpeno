namespace Jumpeno.Shared.Attributes;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class StringValueAttribute(string value) : Attribute {
    public string Value { get; } = value;
}
