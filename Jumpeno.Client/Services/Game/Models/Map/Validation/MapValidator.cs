namespace Jumpeno.Client.Utils;

public static class MapValidator {
    // ID ---------------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateID(int? value, string id = "") {
        var errors = Checker.ValidateUndefined(value, id); value ??= 0;
        Checker.Validate(errors, value < 0, ERROR.INVALID.SetID(id));
        return errors;
    }
    public static int AssertID(int? value, string id = "", AppException? exception = null) {
        return (int)Checker.Assert(value, ValidateID(value, id), exception ?? EXCEPTION.VALUES)!;
    }
}
