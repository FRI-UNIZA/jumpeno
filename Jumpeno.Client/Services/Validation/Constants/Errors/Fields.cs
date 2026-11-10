namespace Jumpeno.Client.Constants;

public static class Fields {
    public static TInfo Default => Invalid;
    public static TInfo Invalid => new("Invalid value");
    public static TInfo Undefined => new("Value undefined");
    public static TInfo Empty => new("Empty field");
    public static TInfo Format => new("Wrong format");
    public static TInfo Match() => new("Equal values");
    public static TInfo Match(object value1, object value2) => new("Equal values of I18N{value1} and I18N{value2}", new() {{ "value1", value1 }, { "value2", value2 } });
    public static TInfo NotMatch() => new("Not a match");
    public static TInfo NotMatch(object value1, object value2) => new("Not a match of I18N{value1} and I18N{value2}", new() {{ "value1", value1 }, { "value2", value2 } });
    public static TInfo Exists => new("Already exists");
}
