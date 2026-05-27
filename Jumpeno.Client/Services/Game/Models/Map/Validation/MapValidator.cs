namespace Jumpeno.Client.Utils;

public static class MapValidator {
    // Name -------------------------------------------------------------------------------------------------------------------------------
    public const int NameMinLength = 3;
    public const int NameMaxLength = 30;

    public static List<Error> ValidateName(string? value, string id = "") {
        var errors = Checker.Validate(value == null, Errors.Undefined.SetID(id));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Trim() == "", Errors.Empty.SetID(id));
        Checker.Validate(errors, value.Length < NameMinLength || NameMaxLength < value.Length,
            Errors.Default.SetID(id)
            .SetInfo("Length is not between I18N{min} and I18N{max}", new() {{ "min", NameMinLength }, { "max", NameMaxLength }})
        );
        return errors;
    }
    public static string AssertName(string? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateName(value, id), exception ?? Exceptions.Values)!;
    }

    // ID ---------------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateID(int? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value ??= 0;
        Checker.Validate(errors, value < 0, Errors.Invalid.SetID(id));
        return errors;
    }
    public static int AssertID(int? value, string id = "", AppException? exception = null) {
        return (int)Checker.Assert(value, ValidateID(value, id), exception ?? Exceptions.Values)!;
    }
}
