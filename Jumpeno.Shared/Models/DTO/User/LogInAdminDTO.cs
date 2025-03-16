namespace Jumpeno.Shared.Models;

public record LogInAdminDTO(
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    string Email
) {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Email { get; init; } = Email.Trim();

    // Field validation -------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateEmail(string email) {
        var errors = Checker.Validate(email == null || email.Length == 0, new Error(nameof(Email), Checker.FIELD_EMPTY));
        if (errors.Count > 0) return errors;
        Checker.Validate(errors, !Checker.IsValidEmail(email!), new Error(nameof(Email), Checker.FIELD_FORMAT));
        return errors;
    }
    public static string CheckEmail(string email, string? message = null) {
        Checker.CheckValues(ValidateEmail(email), message); return email;
    }

    // Object validation ------------------------------------------------------------------------------------------------------------------
    public List<Error> Validate() {
        var errors = ValidateEmail(Email);
        return errors;
    }
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
