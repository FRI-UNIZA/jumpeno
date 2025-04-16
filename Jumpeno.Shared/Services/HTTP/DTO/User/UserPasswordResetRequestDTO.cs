namespace Jumpeno.Shared.Models;

public record UserPasswordResetRequestDTO(
    string Email
) {
    public List<Error> Validate() => UserValidator.ValidateEmail(Email);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
