namespace Jumpeno.Client.Models;

public record AuthRefreshDTOR(
    string AccessToken,
    string RefreshToken
) : IValidable<AuthRefreshDTOR> {
    public List<Error> Validate() {
        List<Error> errors = [];
        errors.AddRange(TokenValidator.ValidateToken(AccessToken, nameof(AccessToken)));
        errors.AddRange(TokenValidator.ValidateToken(RefreshToken, nameof(RefreshToken)));
        return errors;
    }
    public AuthRefreshDTOR Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.SERVER);
}
