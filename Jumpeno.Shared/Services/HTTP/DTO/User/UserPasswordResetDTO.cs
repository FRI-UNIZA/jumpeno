namespace Jumpeno.Shared.Models;

public record UserPasswordResetDTO(
    string ResetToken
) {
    public List<Error> Validate() => TokenValidator.ValidateToken(ResetToken);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
