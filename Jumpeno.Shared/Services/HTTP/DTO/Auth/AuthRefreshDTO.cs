namespace Jumpeno.Shared.Models;

public record AuthRefreshDTO(
    string RefreshToken
) {
    public List<Error> Validate() => TokenValidator.ValidateToken(RefreshToken);
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
