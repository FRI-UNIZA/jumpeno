namespace Jumpeno.Shared.Models;

public record UserLoginDTOR(
    string AccessToken,
    string RefreshToken
) {
    public List<Error> Validate() {
        var errors = TokenValidator.ValidateToken(AccessToken);
        errors.AddRange(TokenValidator.ValidateToken(RefreshToken));
        return errors;
    }
    public void Check(string? message = null) => Checker.CheckValues(Validate(), message);
}
