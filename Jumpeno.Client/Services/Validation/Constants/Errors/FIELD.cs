namespace Jumpeno.Client.Constants;

public static class FIELD {
    public static TInfo DEFAULT => INVALID;
    public static TInfo INVALID => new("Invalid value");
    public static TInfo UNDEFINED => new("Value undefined");
    public static TInfo EMPTY => new("Empty field");
    public static TInfo FORMAT => new("Wrong format");
    public static TInfo MATCH() => new("Equal values");
    public static TInfo MATCH(object value1, object value2) => new("Equal values of I18N{value1} and I18N{value2}", new() {{ "value1", value1 }, { "value2", value2 } });
    public static TInfo NOT_MATCH() => new("Not a match");
    public static TInfo NOT_MATCH(object value1, object value2) => new("Not a match of I18N{value1} and I18N{value2}", new() {{ "value1", value1 }, { "value2", value2 } });
    public static TInfo EXISTS = new("Already exists");
}
