namespace Jumpeno.Shared.Models;

public record UserLoginDTO(
    string Email,
    string Password
) {
    public List<Error> Validate() {
        var errors = UserValidator.ValidateEmail(Email);
        errors.AddRange(UserValidator.ValidatePassword(Password));
        return errors;
    }
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
