namespace Jumpeno.Client.Utils;

public static class MapValidator {
    // Name -------------------------------------------------------------------------------------------------------------------------------
    public const int NAME_MIN_LENGTH = 3;
    public const int NAME_MAX_LENGTH = 30;

    public static List<Error> ValidateName(string? value, string id = "") {
        var errors = Checker.Validate(value == null, Errors.UNDEFINED.SetID(id));
        if (errors.Count > 0) return errors; value = $"{value}";
        Checker.Validate(errors, value.Trim() == "", Errors.EMPTY.SetID(id));
        Checker.Validate(errors, value.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < value.Length,
            Errors.DEFAULT.SetID(id)
            .SetInfo("Length is not between I18N{min} and I18N{max}", new() {{ "min", NAME_MIN_LENGTH }, { "max", NAME_MAX_LENGTH }})
        );
        return errors;
    }
    public static string AssertName(string? value, string id = "", AppException? exception = null) {
        return Checker.Assert(value, ValidateName(value, id), exception ?? Exceptions.VALUES)!;
    }

    // ID ---------------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateID(int? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value ??= 0;
        Checker.Validate(errors, value < 0, Errors.INVALID.SetID(id));
        return errors;
    }
    public static int AssertID(int? value, string id = "", AppException? exception = null) {
        return (int)Checker.Assert(value, ValidateID(value, id), exception ?? Exceptions.VALUES)!;
    }
}
