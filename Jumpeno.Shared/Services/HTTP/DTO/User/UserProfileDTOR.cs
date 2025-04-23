namespace Jumpeno.Shared.Models;

public record UserProfileDTOR(
    User Profile
) {
    public List<Error> Validate() => UserValidator.Validate(Profile);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
