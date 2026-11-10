namespace Jumpeno.Client.Constants;

public static class Errors {
    public static Error Default => Invalid;
    public static Error Invalid => new Error().SetInfo(Fields.Invalid);
    public static Error Undefined => new Error().SetInfo(Fields.Undefined);
    public static Error Empty => new Error().SetInfo(Fields.Empty);
    public static Error Format => new Error().SetInfo(Fields.Format);
    public static Error Match() => new Error().SetInfo(Fields.Match());
    public static Error Match(object value1, object value2) => new Error().SetInfo(Fields.Match(value1, value2));
    public static Error NotMatch() => new Error().SetInfo(Fields.NotMatch());
    public static Error NotMatch(object value1, object value2) => new Error().SetInfo(Fields.NotMatch(value1, value2));
    public static Error Exists => new Error().SetInfo(Fields.Exists);
}
