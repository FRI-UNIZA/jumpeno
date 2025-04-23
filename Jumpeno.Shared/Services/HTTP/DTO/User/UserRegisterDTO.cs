namespace Jumpeno.Shared.Models;

public record UserRegisterDTO(
    string Email,
    string Name,
    string Password
) {
    public List<Error> Validate() {
        var errors = UserValidator.ValidateEmail(Email);
        errors.AddRange(UserValidator.ValidateName(Name));
        errors.AddRange(UserValidator.ValidatePassword(Password));
        return errors;
    }
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
