namespace Jumpeno.Client.Constants;

public static class Errors {
    public static Error DEFAULT => INVALID;
    public static Error INVALID => new Error().SetInfo(Fields.INVALID);
    public static Error UNDEFINED => new Error().SetInfo(Fields.UNDEFINED);
    public static Error EMPTY => new Error().SetInfo(Fields.EMPTY);
    public static Error FORMAT => new Error().SetInfo(Fields.FORMAT);
    public static Error MATCH() => new Error().SetInfo(Fields.MATCH());
    public static Error MATCH(object value1, object value2) => new Error().SetInfo(Fields.MATCH(value1, value2));
    public static Error NOT_MATCH() => new Error().SetInfo(Fields.NOT_MATCH());
    public static Error NOT_MATCH(object value1, object value2) => new Error().SetInfo(Fields.NOT_MATCH(value1, value2));
    public static Error EXISTS => new Error().SetInfo(Fields.EXISTS);
}
