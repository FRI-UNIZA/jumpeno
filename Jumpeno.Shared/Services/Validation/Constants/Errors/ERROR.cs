namespace Jumpeno.Shared.Constants;

public static class ERROR {
    public static Error DEFAULT => INVALID;
    public static Error INVALID => new Error().SetInfo(FIELD.INVALID);
    public static Error UNDEFINED => new Error().SetInfo(FIELD.UNDEFINED);
    public static Error EMPTY => new Error().SetInfo(FIELD.EMPTY);
    public static Error FORMAT => new Error().SetInfo(FIELD.FORMAT);
    public static Error MATCH() => new Error().SetInfo(FIELD.MATCH());
    public static Error MATCH(object value1, object value2) => new Error().SetInfo(FIELD.MATCH(value1, value2));
    public static Error NOT_MATCH() => new Error().SetInfo(FIELD.NOT_MATCH());
    public static Error NOT_MATCH(object value1, object value2) => new Error().SetInfo(FIELD.NOT_MATCH(value1, value2));
    public static Error EXISTS => new Error().SetInfo(FIELD.EXISTS);
}
