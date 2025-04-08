namespace Jumpeno.Shared.Models;

public record UserActivateDTO(
    string ActivationToken
) {
    // Validation -------------------------------------------------------------------------------------------------------------------------
    public List<Error> Validate() => TokenValidator.ValidateToken(ActivationToken);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
