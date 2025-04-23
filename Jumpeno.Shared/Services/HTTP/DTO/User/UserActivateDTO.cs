namespace Jumpeno.Shared.Models;

public record UserActivateDTO(
    string ActivationToken
) {
    public List<Error> Validate() => TokenValidator.ValidateToken(ActivationToken);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
