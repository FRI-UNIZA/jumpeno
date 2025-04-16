namespace Jumpeno.Shared.Models;

public record AdminLoginDTO(
    string Email
) {
    public List<Error> Validate() => AdminValidator.ValidateEmail(Email);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
