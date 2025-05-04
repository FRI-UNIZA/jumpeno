namespace Jumpeno.Shared.Models;

public record UserLoginDTOR(
    string AccessToken,
    string RefreshToken
) : IValidable<UserLoginDTOR> {
    public List<Error> Validate() {
        var errors = TokenValidator.ValidateToken(AccessToken, nameof(AccessToken));
        errors.AddRange(TokenValidator.ValidateToken(RefreshToken, nameof(RefreshToken)));
        return errors;
    }
    public UserLoginDTOR Assert(AppException? exception = null) => Checker.AssertWith(this, Validate(), exception ?? EXCEPTION.SERVER);
}
